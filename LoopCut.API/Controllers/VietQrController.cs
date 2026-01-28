using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/vietqr")]
public class VietQRController : ControllerBase
{
    private readonly VietQRService _vietQRService;

    public VietQRController(VietQRService vietQRService)
    {
        _vietQRService = vietQRService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateQR([FromBody] VietQRGenerateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AccountNo))
                return BadRequest(new { message = "Số tài khoản không được để trống" });

            if (request.AcqId <= 0)
                return BadRequest(new { message = "Mã ngân hàng không hợp lệ" });

            var result = await _vietQRService.GenerateQRCodeAsync(request);

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}