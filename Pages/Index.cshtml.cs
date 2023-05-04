using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace spl.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string username, string password)
        {
            //add asp-page-handler="login" to form if want to use named method
            Debug.WriteLine($"Index OnPost: Logging user {username}");
            Response.Cookies.Append("UserType", "admin");
            Response.Cookies.Append("IsAuthenticated", "1");

            return RedirectToPage("/home/dashboard");
        }
    }
}