using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.FilterLogDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoopCut.API.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }
        [HttpGet]
        public async Task<IActionResult> GetLogs(int pageIndex = 1 , int pageSize = 10)
        {
            var logs = await _logService.GetAllLogsAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<AuditLogging>>.OkResponse(logs,"Get successful","200"));
        }

    }
}
