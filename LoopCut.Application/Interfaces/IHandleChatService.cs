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
        Task<string> HandleMembershipUserAsync(AiCommand command);
        Task<string> HandleMembershipInfoAsync(AiCommand command);
        Task<string> HandleUnknownAsync(AiCommand command);
        Task<string> HandleSubscriptionAsync(AiCommand command);
    }

}
