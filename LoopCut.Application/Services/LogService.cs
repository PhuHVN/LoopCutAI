using LoopCut.Application.DTOs.FilterLogDtos;
using LoopCut.Application.Interfaces;
using LoopCut.Domain.Abstractions;
using LoopCut.Domain.Entities;
using LoopCut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class LogService : ILogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly JsonSerializerOptions _jsonOptions;

        public LogService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<BasePaginatedList<AuditLogging>> GetAllLogsAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<AuditLogging>().Entity;
            var logs = await _unitOfWork.GetRepository<AuditLogging>().GetPagging(query, pageIndex, pageSize);
            return logs;
        }

        public async Task<BasePaginatedList<AuditLogging>> GetLogsByFilterAsync(int pageIndex, int pageSize, FilterLogDto filter)
        {
            var query = _unitOfWork.GetRepository<AuditLogging>().Entity;

            // Filter by UserId
            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                query = query.Where(x => x.UserId == filter.UserId);
            }

            // Filter by Action
            if (filter.Action.HasValue)
            {
                query = query.Where(x => x.Action == filter.Action.Value);
            }

            // Filter by EntityName
            if (!string.IsNullOrWhiteSpace(filter.EntityName))
            {
                query = query.Where(x => x.EntityName == filter.EntityName);
            }

            // Filter by EntityId
            if (!string.IsNullOrWhiteSpace(filter.EntityId))
            {
                query = query.Where(x => x.EntityId == filter.EntityId);
            }

            // Filter by Date Range
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                // Include entire day
                var endDate = filter.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedAt <= endDate);
            }

            // Filter by Status
            if (filter.Status.HasValue)
            {
                query = query.Where(x => x.Status == filter.Status.Value);
            }

            // Filter by IpAddress
            if (!string.IsNullOrWhiteSpace(filter.IpAddress))
            {
                query = query.Where(x => x.IpAddress == filter.IpAddress);
            }

            // Search in OldValues/NewValues (optional)
            if (!string.IsNullOrWhiteSpace(filter.SearchInValues))
            {
                query = query.Where(x =>
                    (x.OldValues != null && x.OldValues.Contains(filter.SearchInValues)) ||
                    (x.NewValues != null && x.NewValues.Contains(filter.SearchInValues))
                );
            }

            // Order by CreatedAt descending (newest first)
            query = query.OrderByDescending(x => x.CreatedAt);

            return await _unitOfWork.GetRepository<AuditLogging>()
                .GetPagging(query, pageIndex, pageSize);
        }

        public async Task LogAsync<T>(AuditActionEnum action,string entityName,string entityId,T? oldValues = default,T? newValues = default)
        {
            var userId = _userService.GetCurrentUserId();
            var getIpAddress = _userService.GetIpAddress();
            await LogAsync(
            userId: userId,
            action: action,
            entityName: entityName,
            entityId: entityId,
            oldValues: oldValues,
            newValues: newValues,
            ipAddress: getIpAddress
        );
        }

        public async Task LogAsync<T>(
        string? userId,
        AuditActionEnum action,
        string entityName,
        string entityId,
        T? oldValues = default,
        T? newValues = default,
        string? ipAddress = null)
        {
            var log = new AuditLogging
            {
                UserId = userId ?? "Unknow" ,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                OldValues = oldValues != null
                    ? JsonSerializer.Serialize(oldValues, _jsonOptions)
                    : string.Empty,
                NewValues = newValues != null
                    ? JsonSerializer.Serialize(newValues, _jsonOptions)
                    : string.Empty,
                IpAddress = ipAddress ?? "",
                CreatedAt = DateTime.UtcNow,
                Status = StatusEnum.Active
            };

            await _unitOfWork.GetRepository<AuditLogging>().InsertAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
