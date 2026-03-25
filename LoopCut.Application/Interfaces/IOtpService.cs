using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface IOtpService
    {
        Task StoreOtpAsync(string key, string otp, TimeSpan expiration);
        Task<string?> RetrieveOtpAsync(string key);
        Task RemoveOtpAsync(string key);
        string GenerateOTP();
    }
}
