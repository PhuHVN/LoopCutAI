using LoopCut.Application.DTOs.AICommand;
using LoopCut.Application.DTOs.PaymentDTO;
using LoopCut.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class HandleChatService : IHandleChatService
    {
        private readonly IUserService user;
        private readonly IMembershipService _membershipService;
        private readonly IPaymentService _paymentService;
        private readonly IUserMembershipService _userMembershipService;
        private readonly ISubscriptionService _subscriptionService;

        public HandleChatService(IUserService userService, IMembershipService membershipService, IPaymentService paymentService, IUserMembershipService userMembershipService, ISubscriptionService subscriptionService)
        {
            user = userService;
            _membershipService = membershipService;
            _paymentService = paymentService;
            _userMembershipService = userMembershipService;
            _subscriptionService = subscriptionService;
        }

        public Task<string> HandleMembershipCompareAsync(AiCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task<string> HandleMembershipInfoAsync(AiCommand command)
        {
            if (!command.Data.TryGetProperty("name", out var membershipNameProp))
            {
                return "Mình cần biết tên gói membership bạn muốn tìm hiểu nha. Bạn có thể cho mình biết tên gói không?";
            }

            var membershipName = membershipNameProp.GetString();
            if (string.IsNullOrEmpty(membershipName))
            {
                return "Hmm, có vẻ tên gói bạn nhập chưa đúng. Bạn thử nhập lại tên gói membership giúp mình nhé!";
            }

            var memberships = await _membershipService.GetMembershipByName(membershipName);

            if (memberships == null)
            {
                return $"Mình không tìm thấy gói '{membershipName}' trong hệ thống. " +
                       $"Bạn có thể kiểm tra lại tên gói hoặc hỏi mình về các gói membership có sẵn nha!";
            }

            return $"Mình tìm thấy rồi! Đây là thông tin gói {memberships.Name}:\n\n" +
                   $"Giá: {memberships.Price:N0} VNĐ/tháng\n" +
                   $"Mô tả: {memberships.Description}\n\n" +
                   $"Thời gian: {memberships.DurationInMonths} tháng\n\n" +
                   $"Bạn có muốn đăng ký gói này không?";
        }

        public async Task<string> HandleMembershipListAsync(AiCommand command)
        {
            var memberships = await _membershipService.GetAllMemberships(1,10);
            if (memberships.TotalItems == 0)
            {
                return "Hiện tại mình không tìm thấy gói membership nào trong hệ thống. " +
                       "Bạn có muốn mình tư vấn về các gói membership khi có sẵn không?";
            }
            var response = "Đây là danh sách các gói membership hiện có:\n\n";
            foreach (var item in memberships.Items)
            {
                response += $"- {item.Name}: {item.Price:N0} VNĐ/tháng\n" +
                            $"  Mô tả: {item.Description}\n\n" +
                            $"  Thời gian: {item.DurationInMonths} tháng\n\n";
            }
            response += "Bạn có muốn mình tư vấn thêm về các gói này không?";
            return response;
        }

        public async Task<string> HandleMembershipUserAsync(AiCommand command)
        {
            var userInfo = await user.GetCurrentUserLoginAsync();

            if (userInfo == null)
            {
                return "Ối, có vẻ bạn chưa đăng nhập. Bạn cần đăng nhập trước để mình có thể kiểm tra thông tin membership của bạn nhé!";
            }

            var userId = userInfo.Id;
            var userMemberships = await _userMembershipService.GetActiveMembershipByUserId(userId);

            if (userMemberships == null)
            {
                return "Hiện tại bạn chưa có gói membership nào đang hoạt động. " +
                       "Bạn có muốn mình tư vấn về các gói membership phù hợp không?";
            }

            var isActive = userMemberships.EndDate >= DateTime.UtcNow;
            var statusText = isActive ? "Đang hoạt động" : "Đã hết hạn";

            var response = $"Đây là thông tin membership của bạn:\n\n" +
                          $"Gói: {userMemberships.Membership.Name}\n" +
                          $"Giá: {userMemberships.Membership.Price:N0} VNĐ/tháng\n" +
                          $"Thời lượng: {userMemberships.Membership.DurationInMonths} tháng\n" +
                          $"Thời gian: {userMemberships.StartDate:dd/MM/yyyy} đến {userMemberships.EndDate:dd/MM/yyyy}\n" +
                          $"Trạng thái: {statusText}";

            if (!isActive)
            {
                response += "\n\nGói của bạn đã hết hạn rồi. Bạn có muốn gia hạn không?";
            }
            else
            {
                var daysLeft = (userMemberships.EndDate - DateTime.UtcNow).Days;
                if (daysLeft <= 7)
                {
                    response += $"\n\nGói của bạn sắp hết hạn trong {daysLeft} ngày nữa. Bạn có muốn gia hạn trước không?";
                }
            }

            return response;
        }

        public async Task<string> HandlePaymentHistoryAsync(AiCommand command)
        {
            var userInfo = await user.GetCurrentUserLoginAsync();
            if (userInfo == null)
            {
                return "Bạn cần đăng nhập để mình có thể kiểm tra lịch sử thanh toán của bạn nhé!";
            }
            var userId = userInfo.Id;
            var filter = new PaymentFilterRequest
            {
                UserId = userId
            };
            var payments = await _paymentService.GetAllPayments(1, 5,filter);
            if (payments.TotalItems == 0)
            {
                return "Mình không tìm thấy giao dịch thanh toán nào trong lịch sử của bạn. " +
                       "Bạn có muốn mình tư vấn về các gói membership và cách thanh toán không?";
            }
            var response = "Đây là lịch sử thanh toán gần đây của bạn:\n\n";
            foreach (var item in payments.Items)
            {
                response += $"- Mã đơn hàng: {item.OrderId}\n" +
                            $"- Gói: {item.MembershipName}\n" +
                            $"- Số tiền: {item.Price:N0} VNĐ\n" +
                            $"- Ngày thanh toán: {item.CreatedAt:dd/MM/yyyy}\n" +
                            $"- Trạng thái: {item.Status}\n\n";
            }
            return response;
        }

        public async Task<string> HandleSubscriptionHistoryAsync(AiCommand command)
        {
            var subscription = await _subscriptionService.GetAllSubscriptionsByUserLoginAsync(1,10);
            if (subscription == null) {
                return $"Bạn có thể kiểm tra lại email hoặc hỏi mình về các gói đăng ký có sẵn nha!";
            }
            var response = $"Mình tìm thấy rồi! Đây là thông tin gói đăng ký của bạn:\n\n";
            foreach (var item in subscription.Items)
            {
                response += $"- Tên gói đăng ký: {item.SubscriptionsName}\n" +
                            $"- Giá: {item.Price:N0} VNĐ\n" +
                            $"- Ngày bắt đầu: {item.StartDate:dd/MM/yyyy}\n" +
                            $"- Ngày kết thúc: {(item.EndDate.HasValue ? item.EndDate.Value.ToString("dd/MM/yyyy") : "Vô hạn")}\n" +
                            $"- Trạng thái: {item.Status}\n\n";
            }
            return response;
        }

        public Task<string> HandleSubscriptionPlansAsync(AiCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task<string> HandleUnknownAsync(AiCommand command)
        {
            var intent = "default";
            if (command.Data.TryGetProperty("intent", out var intentProp))
            {
                intent = intentProp.GetString() ?? "default";
            }

            return intent switch
            {
                "greeting" => "Chào bạn! Mình là trợ lý ảo LoopCut, chuyên hỗ trợ về membership và subscription. " +
                             "Bạn muốn kiểm tra gói đăng ký của mình hoặc tìm hiểu về các gói có sẵn không?",

                "thanks" => "Không có gì đâu! Rất vui được hỗ trợ bạn. Nếu có thêm câu hỏi gì, cứ hỏi mình nhé!",

                "goodbye" => "Tạm biệt bạn! Chúc bạn một ngày tuyệt vời! Nếu cần hỗ trợ gì, quay lại chat với mình bất cứ lúc nào nhé!",

                "offtopic" => "Ồ, câu hỏi này hơi nằm ngoài chuyên môn của mình rồi! " +
                             "Mình chỉ giỏi về membership và subscription thôi. Bạn có câu hỏi gì về các gói đăng ký không?",

                "compliment" => "Cảm ơn bạn! Mình sẽ cố gắng hỗ trợ bạn tốt nhất có thể!",

                "complaint" => "Mình xin lỗi vì trải nghiệm chưa tốt. Bạn có thể cho mình biết cụ thể vấn đề để mình hỗ trợ tốt hơn không?",

                _ => "Hmm, mình chưa hiểu rõ ý bạn lắm. Bạn có thể hỏi mình về:\n" +
                     "- Thông tin các gói membership\n" +
                     "- Kiểm tra gói đăng ký của bạn\n" +
                     "- Gia hạn hoặc nâng cấp gói\n\n" +
                     "Bạn muốn biết thông tin gì nhất?"
            };
        }
    }
}