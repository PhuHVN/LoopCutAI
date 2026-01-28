namespace LoopCut.Application.DTOs.ServiceDTO
{
    public class ServiceUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
    }
}
