using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace spl.Pages
{
    public class DashboardModel : PageModel
    {
        public string? Layout { get; private set; }

        public void OnGet()
        {
            // Get user type from session variable
            string userType = Request.Cookies["UserType"] ?? "";

            if (userType == "admin")
            {
                Layout = "Shared/_AdminLayout.cshtml";
            }
            else
            {
                Layout = "Shared/_UrusetiaLayout.cshtml";
            }
        }
    }
}
