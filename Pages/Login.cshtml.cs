using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Services;

namespace Week5.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userService.AuthenticateUserAsync(Username, Password);

            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            // Generate a simple token
            string token = GenerateToken();
            string sessionId = HttpContext.Session.Id;

            // Store values in session
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", sessionId);
            HttpContext.Session.SetString("role", user.Role);

            // Store values in cookies
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", sessionId, cookieOptions);

            // Redirect to the table page
            return RedirectToPage("/Index");
        }

        private string GenerateToken()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}