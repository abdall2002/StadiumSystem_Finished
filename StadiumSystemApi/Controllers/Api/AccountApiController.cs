using Microsoft.AspNetCore.Mvc;

namespace StatiumSystemApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        [HttpGet("IsAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(User.Identity.IsAuthenticated);
        }
    }
}
