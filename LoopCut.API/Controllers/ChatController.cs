using LoopCut.Application.DTOs.ChatDTO;
using LoopCut.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/chat")]
    [Authorize]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly GeminiService _geminiService;
        public ChatController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest message)
        {
            try
            {
                var response = await _geminiService.SendMessageAsync(message.Message);
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}
