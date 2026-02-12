using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobseeker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetDashboard()
        {
            var mobile = User.Identity.Name; // JWT se mobile milega

            return Ok(new
            {
                message = "Welcome to Dashboard",
                user = mobile
            });
        }
    }
}
