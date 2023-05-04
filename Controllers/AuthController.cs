using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace spl.Controllers
{
    public class AuthController : Controller
    {
        [HttpPost("auth/logout")]
        public IActionResult Logout()
        {
            Debug.WriteLine("AuthController: Logging out");

            Response.Cookies.Delete("UserType");
            Response.Cookies.Delete("IsAuthenticated");

            return Ok();
        }
    }
}
