using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Week5.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Remove cookies
            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // Redirect to login page
            return RedirectToPage("/Login");
        }
    }
}