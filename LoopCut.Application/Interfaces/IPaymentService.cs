using LoopCut.Application.DTOs.PaymentDTO;
using LoopCut.Application.DTOs.PayOsDto;
using LoopCut.Domain.Abstractions;
using PayOS.Models.Webhooks;
using PayOS.Resources.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponsed> CreatePaymentLink(CreatePaymentRequest request);
        Task<bool> VerifyPaymentWebhook(Webhook webhookData);
        Task<PaymentDetailResponse> GetPaymentInfo(string orderCode);
        Task<BasePaginatedList<PaymentDetailResponse>> GetAllPayments(int pageIndex, int pageSize, PaymentFilterRequest filter);
    }
}
