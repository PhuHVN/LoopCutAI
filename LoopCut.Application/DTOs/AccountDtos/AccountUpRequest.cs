using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.DTOs.AccountDtos
{
    public class AccountUpRequest
    {   
        public string FullName { get; set; } = string.Empty;
        public IFormFile? AvatarUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
