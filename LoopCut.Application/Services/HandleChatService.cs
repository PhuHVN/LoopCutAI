using LoopCut.Application.DTOs.AICommand;
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
        private readonly IUserMembershipService _userMembershipService;
        public HandleChatService( IUserService userService, IUserMembershipService userMembershipService)
        {
            user = userService;
            _userMembershipService = userMembershipService;
        }
        public async Task<string> HandleMembershipInfoAsync(AiCommand command)
        {
            if (!command.Data.TryGetProperty("email", out var emailProp))
                return "Bạn vui lòng cung cấp email để kiểm tra subscription.";

            var email = emailProp.GetString();

            if (string.IsNullOrEmpty(email))
                return "Email không hợp lệ.";

            return "Chưa triển khai kiểm tra thông tin đăng ký.";

        }

        public async Task<string> HandleMembershipUserAsync(AiCommand command)
        {
            var userInfo = await user.GetCurrentUserLoginAsync();

            if (userInfo == null)
                return "Bạn cần đăng nhập để xem thông tin membership.";

            var userId = userInfo.Id;
            var userMemberships = await _userMembershipService.GetActiveMembershipByUserId(userId);

            if (userMemberships == null)
            {
                return "Bạn chưa đăng ký membership nào. Vui lòng đăng ký để sử dụng dịch vụ.";
            }
            return
                $"Membership: {userMemberships.Membership.Name} " +
                $"- Giá: {userMemberships.Membership.Price:N0} VND/tháng " +
                $"- Thời gian: {userMemberships.StartDate:dd/MM/yyyy} " +
                $"- {userMemberships.EndDate:dd/MM/yyyy} " +
                $"- Trạng thái: {(userMemberships.EndDate >= DateTime.UtcNow ? "Đang hoạt động" : "Hết hạn")}";
        }

        public async Task<string> HandleSubscriptionAsync(AiCommand command)
        {
            if (!command.Data.TryGetProperty("email", out var emailProp))
                return "Bạn vui lòng cung cấp email để kiểm tra subscription.";

            var email = emailProp.GetString();

            if (string.IsNullOrEmpty(email))
                return "Email không hợp lệ.";

            return "Chưa triển khai kiểm tra thông tin đăng ký.";
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
                "greeting" => "Xin chào! Mình là trợ lý hỗ trợ membership. Bạn muốn kiểm tra gói đăng ký của mình không?",
                "thanks" => "Không có gì! Rất vui được hỗ trợ bạn!",
                "goodbye" => "Tạm biệt! Chúc bạn một ngày tốt lành! ",
                "offtopic" => "Mình chỉ hỗ trợ các vấn đề về membership và subscription nhé! Bạn có câu hỏi nào về gói đăng ký không?",
                _ => "Mình chưa hiểu rõ câu hỏi của bạn. Bạn có thể hỏi về membership hoặc subscription không? "
            };
        }
    }
}
