using LoopCut.Application.DTOs.AICommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IHandleChatService
    {
        //Handle Membership
        Task<string> HandleMembershipUserAsync(AiCommand command); 
        Task<string> HandleMembershipListAsync(AiCommand command);
        Task<string> HandleMembershipCompareAsync(AiCommand command);
        Task<string> HandleMembershipInfoAsync(AiCommand command);
        //Handle Subscription
        Task<string> HandleSubscriptionHistoryAsync(AiCommand command);
        Task<string> HandleSubscriptionStatusAsync(AiCommand command);
        //Handle Unknown
        Task<string> HandleUnknownAsync(AiCommand command);
        //Handle Payment
        Task<string> HandlePaymentHistoryAsync(AiCommand command);
    }

}
