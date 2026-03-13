using AutoMapper;
using LoopCut.Application.DTOs.PaymentDTO;
using LoopCut.Application.DTOs.PayOsDto;
using LoopCut.Application.DTOs.UserMembershipDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using PayOS.Resources.Webhooks;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOSClient _payOSClient;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly IUserMembershipService _userMembershipService;
        private readonly ILogService _logService;
        private readonly IMapper mapper;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, ILogger<PaymentService> logger, IMapper mapper, IUserMembershipService userMembershipService, ILogService logService, IPaymentRepository paymentRepository)
        {
            _payOSClient = new PayOSClient(
                configuration["PayOS:ClientId"]!,
                configuration["PayOS:ApiKey"]!,
                configuration["PayOS:ChecksumKey"]!
            );
            this.unitOfWork = unitOfWork;
            _configuration = configuration;
            _userMembershipService = userMembershipService;
            _logger = logger;
            this.mapper = mapper;
            _logService = logService;
            _paymentRepository = paymentRepository;
        }


        public async Task<PaymentResponsed> CreatePaymentLink(CreatePaymentRequest request)
        {
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                 + Random.Shared.Next(100, 999);

            var user = await unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Id == request.UserId && x.Status == StatusEnum.Active);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var membership = await unitOfWork.GetRepository<Membership>().FindAsync(x => x.Id == request.MembershipId && x.Status == StatusEnum.Active);
            if (membership == null)
            {
                throw new KeyNotFoundException("Membership not found");
            }

            string baseUrl = !string.IsNullOrEmpty(request.ReturnUrlDomain)
                ? request.ReturnUrlDomain : _configuration["PayOs:Url"]!;

            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = (long)membership.Price,
                Description = $"{membership.Name}_{membership.Price}",
                ReturnUrl = $"{baseUrl}/payment/success",
                CancelUrl = $"{baseUrl}/payment/cancel"
            };
            var payment = new Payment
            {
                UserId = request.UserId,
                MembershipId = request.MembershipId,
                Amount = (int)membership.Price,
                OrderCode = orderCode.ToString(),
                Description = paymentRequest.Description,
                Status = PaymentStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow,
            };
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _payOSClient.PaymentRequests.CreateAsync(paymentRequest);

                await unitOfWork.GetRepository<Payment>().InsertAsync(payment);
                await unitOfWork.SaveChangesAsync();
                await _logService.LogAsync(
                    action: AuditActionEnum.Create,
                    entityName: nameof(Payment),
                    entityId: payment.Id,
                    newValues: $"Created payment with OrderCode: {payment.OrderCode}, UserId: {payment.UserId}, MembershipId: {payment.MembershipId}, Amount: {payment.Amount}, Status: {payment.Status}"
                );
                await unitOfWork.CommitTransactionAsync();
                return new PaymentResponsed
                {
                    CheckoutUrl = result.CheckoutUrl,
                    OrderCode = result.OrderCode,
                    Message = "Payment link created successfully"
                };

            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackTransactionAsync();
                _logger.LogError(ex, "Failed to create payment link for UserId: {UserId}, MembershipId: {MembershipId}",
                    request.UserId, request.MembershipId);
                throw new InvalidOperationException("Failed to create payment link. Please try again later.");
            }
        }

        public async Task<PaymentDetailResponse> GetPaymentInfo(string OrderCode)
        {
            var payment = await unitOfWork.GetRepository<Payment>().FindAsync(x => x.OrderCode == OrderCode, include: q => q.Include(u => u.User).Include(m => m.Membership));

            if (payment == null)
                throw new KeyNotFoundException("Payment not found");

            if (payment.User == null)
                throw new KeyNotFoundException("User not found");

            if (payment.Membership == null)
                throw new KeyNotFoundException("Membership not found");

            return new PaymentDetailResponse
            {
                OrderId = payment.OrderCode,
                UserId = payment.User.Id,
                Email = payment.User.Email,
                FullName = payment.User.FullName,
                MembershipId = payment.Membership.Id,
                MembershipName = payment.Membership.Name,
                Price = payment.Amount,
                Description = payment.Description,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt,
                UpdateAt = payment.UpdatedAt
            };
        }

        public async Task<bool> VerifyPaymentWebhook(Webhook webhookData)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var data = await _payOSClient.Webhooks.VerifyAsync(webhookData);
                var orderCodeStr = data.OrderCode.ToString();
                var existingPayment = await _paymentRepository.GetPaymentForUpdateAsync(orderCodeStr);

                if (existingPayment == null)
                {
                    _logger.LogWarning("Payment not found for OrderCode: {OrderCode}", data.OrderCode);
                    await unitOfWork.RollBackTransactionAsync();
                    return false;
                }

                if (existingPayment.Status == PaymentStatusEnum.Completed ||
                    existingPayment.Status == PaymentStatusEnum.Failed)
                {
                    _logger.LogWarning("Webhook already processed for OrderCode: {OrderCode}. Current status: {Status}",
                        data.OrderCode, existingPayment.Status);
                    await unitOfWork.CommitTransactionAsync();
                    return true;
                }

                switch (data.Code)
                {
                    case "00":
                        await UpdatePaymentWithTransaction(existingPayment, PaymentStatusEnum.Completed);
                        _logger.LogInformation("Payment completed for OrderCode: {OrderCode}", data.OrderCode);
                        break;
                    case "01":
                        await UpdatePaymentWithTransaction(existingPayment, PaymentStatusEnum.Failed);
                        _logger.LogInformation("Payment failed for OrderCode: {OrderCode}", data.OrderCode);
                        break;
                    case "02":
                        await UpdatePaymentWithTransaction(existingPayment, PaymentStatusEnum.Process);
                        _logger.LogInformation("Payment processing for OrderCode: {OrderCode}", data.OrderCode);
                        break;
                    default:
                        _logger.LogError("Unknown payment status code: {Code} for OrderCode: {OrderCode}",
                            data.Code, data.OrderCode);
                        await unitOfWork.RollBackTransactionAsync();
                        return false;
                }
                await _logService.LogAsync(
                    action: AuditActionEnum.Update,
                    entityName: nameof(Payment),
                    entityId: existingPayment.Id,
                    newValues: $"Updated payment status to {existingPayment.Status} for OrderCode: {existingPayment.OrderCode}"
                );
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackTransactionAsync();
                _logger.LogError(ex, "PayOS webhook verification failed for OrderCode: {OrderCode}",
                    webhookData?.Code);
                return false;
            }
        }

        private async Task UpdatePaymentWithTransaction(Payment payment, PaymentStatusEnum status)
        {
            if (status == PaymentStatusEnum.Completed &&
                payment.Status != PaymentStatusEnum.Completed)
            {
                try
                {
                    var userMembershipReq = new UserMembershipRequest
                    {
                        UserId = payment.UserId,
                        MembershipId = payment.MembershipId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(payment.Membership.DurationInMonths)
                    };

                    await _userMembershipService.AssignMembershipToUser(userMembershipReq);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("already has an active membership"))
                {
                    _logger.LogWarning("User already has active membership.");
                }
            }

            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Updating payment status to {Status} for OrderCode: {OrderCode}", status, payment.OrderCode);
            await unitOfWork.GetRepository<Payment>().UpdateAsync(payment);
            await unitOfWork.SaveChangesAsync();
        }
        public async Task<BasePaginatedList<PaymentDetailResponse>> GetAllPayments(int pageIndex, int pageSize, PaymentFilterRequest filter)
        {
            await unitOfWork.BeginTransactionAsync();  
            try
            {
                var query = unitOfWork.GetRepository<Payment>().Entity.AsQueryable();

                if (!string.IsNullOrEmpty(filter.UserId))
                {
                    query = query.Where(x => x.UserId == filter.UserId);
                }
                if (filter.Status.HasValue)
                {
                    query = query.Where(x => x.Status == filter.Status.Value);
                }
                if (filter.DateFrom.HasValue)
                {
                    query = query.Where(x => x.CreatedAt >= filter.DateFrom.Value);
                }
                if (filter.DateTo.HasValue)
                {
                    query = query.Where(x => x.CreatedAt <= filter.DateTo.Value);
                }
                if (!string.IsNullOrEmpty(filter.MembershipId))
                {
                    query = query.Where(x => x.MembershipId == filter.MembershipId);
                }
                if (!string.IsNullOrEmpty(filter.OrderCode))
                {
                    query = query.Where(x => x.OrderCode == filter.OrderCode);
                }
                if (!string.IsNullOrEmpty(filter.Email))
                {
                    query = query.Where(x => x.User.Email == filter.Email);
                }
                query = query.Include(x => x.User).Include(x => x.Membership).OrderByDescending(x => x.CreatedAt);

                var payments = await unitOfWork.GetRepository<Payment>()
                    .GetPagging(query, pageIndex, pageSize);

                await unitOfWork.CommitTransactionAsync();  

                return mapper.Map<BasePaginatedList<PaymentDetailResponse>>(payments);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackTransactionAsync(); 
                _logger.LogError(ex, "Failed to get all payments");
                throw;
            }
        }
    }

}
