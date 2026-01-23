using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Domain.Abstractions;

namespace LoopCut.Application.Interfaces
{
    public interface IServiceDefinitionManager
    {
        Task<ServiceResponse> CreateService(ServiceRequestV1 serviceRequest);
        Task<ServiceResponse> UpdateService(string id, ServiceRequestV1 serviceRequest);
        Task<ServiceResponse> GetServiceById(string id);
        Task DeleteService(string id);
    }
}
