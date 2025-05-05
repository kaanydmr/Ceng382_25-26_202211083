using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Week5.Data;
using Week5.Models;

namespace Week5.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        public IList<Class> ClassList { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string StatusMessage { get; set; }

        [BindProperty]
        public Class Class { get; set; }

        // GET: Read all classes from database
        public async Task<IActionResult> OnGetAsync(string status = null)
        {
            // Check if user is authenticated
            Username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToPage("/Login");
            }

            Role = HttpContext.Session.GetString("role");
            
            // Get classes from database
            ClassList = await _context.Classes.ToListAsync();
            
            if (!string.IsNullOrEmpty(status))
            {
                StatusMessage = status;
            }
            
            return Page();
        }
        
        // POST: Create a new class in database
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                ClassList = await _context.Classes.ToListAsync();
                Username = HttpContext.Session.GetString("username");
                Role = HttpContext.Session.GetString("role");
                return Page();
            }

            // Add class to database
            _context.Classes.Add(Class);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { status = "Class created successfully!" });
        }

        // POST: Update existing class in database
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                ClassList = await _context.Classes.ToListAsync();
                Username = HttpContext.Session.GetString("username");
                Role = HttpContext.Session.GetString("role");
                return Page();
            }

            // Update class in database
            _context.Attach(Class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(Class.Id))
                {
                    return RedirectToPage("./Index", new { status = "Error: Class not found!" });
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { status = "Class updated successfully!" });
        }

        // POST: Delete class from database
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Find class in database
            var classToDelete = await _context.Classes.FindAsync(id);

            if (classToDelete == null)
            {
                return RedirectToPage("./Index", new { status = "Error: Class not found!" });
            }

            // Remove class from database
            _context.Classes.Remove(classToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { status = "Class deleted successfully!" });
        }
        
        // GET: Retrieve class details from database for editing
        public async Task<JsonResult> OnGetClassDetailsAsync(int id)
        {
            // Find class in database
            var classItem = await _context.Classes.FindAsync(id);
            
            if (classItem == null)
            {
                return new JsonResult(new { success = false, message = "Class not found" });
            }
            
            return new JsonResult(new { success = true, data = classItem });
        }
        
        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}