using LoopCut.Application.DTOs.ServiceDTO;
using LoopCut.Application.DTOs.ServicePlanDTO;

namespace LoopCut.Application.Interfaces
{
    public interface IServicePlanManager
    {
        Task<ServicePlanResponse> GetServicePlansByIdAsync(string id);
        Task<ServicePlanResponse> UpdateServicePlanAsync(string id, ServicePlanRequestV1 servicePlanRequestV1);

        Task<ServicePlanResponse> DeleteServicePlanAsync(string id);
    }
}
