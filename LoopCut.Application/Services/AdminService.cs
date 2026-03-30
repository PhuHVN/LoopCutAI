using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class AdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminService> _logger;
        private readonly ILogService _logService;
        private readonly IPaymentService _paymentService;

        public AdminService(IUnitOfWork unitOfWork, ILogger<AdminService> logger, ILogService logService, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _logService = logService;
            _paymentService = paymentService;
        }
    } 
}
