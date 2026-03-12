using LoopCut.Application.DTOs.FilterLogDtos;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Interfaces
{
    public interface ILogService
    {
        Task LogAsync<T>(AuditActionEnum action,string entityName, string entityId,T? oldValues = default,T? newValues = default);
        Task<BasePaginatedList<LogResponse>> GetAllLogsAsync(int pageIndex, int pageSize);
        Task<BasePaginatedList<LogResponse>> GetLogsByFilterAsync(int pageIndex, int pageSize, FilterLogDto dto);
        
    }
}
