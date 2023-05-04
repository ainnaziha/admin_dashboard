using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace spl.Pages.Division
{
    public class BranchModel : PageModel
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

        public void OnGetFetchBranch()
        {
            Debug.WriteLine("Branch OnGetFetchBranch: Fetch branch list");

        }

        public void OnGetFetchSection()
        {
            Debug.WriteLine("Branch OnGetFetchSection: Fetch section list");

        }

        public void OnGetFetchUnit()
        {
            Debug.WriteLine("Branch OnGetFetchUnit: Fetch unit list");

        }
    }
}
