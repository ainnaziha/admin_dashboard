using Microsoft.AspNetCore.Mvc.RazorPages;

namespace spl.Pages.Division
{
    public class StationModel : PageModel
    {
        public string? Layout { get; private set; }

        public void OnGet()
        {
            string userType = Request.Cookies["UserType"] ?? "";

            if (userType == "admin")
            {
                Layout = "../Shared/_AdminLayout.cshtml";
            }
            else
            {
                Layout = "../Shared/_UrusetiaLayout.cshtml";
            }
        }
    }
}
