using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private AiService aiService = new AiService();

        [HttpPost("ask")]
        public IActionResult Ask(AskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Вопрос не может быть пустым.");
            }

            var answer = aiService.Ask(request.Question);

            return Ok(answer);
        }
    }
}