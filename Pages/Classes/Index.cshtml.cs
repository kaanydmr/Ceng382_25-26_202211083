using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Week5.Data;
using Week5.Models;
using System.Text.Json;
using System.Text.Json; 
using System.Text.Json.Serialization; 
using System.IO; 
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Week5.Pages.Classes
{
    [Authorize]
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


        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        private const int PageSize = 10;
        private static int counter = 0;


       public async Task<IActionResult> OnGetAsync()
{
    const int pageSize = 10;

    // Read query string for page and search
    int page = 1;
    string search = Request.Query["search"];

    if (int.TryParse(Request.Query["page"], out var parsedPage))
    {
        page = parsedPage;
    }

    var query = _context.Classes.AsQueryable();

    // Apply search if provided
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(c => c.Name.Contains(search) ||
                                 c.Description.Contains(search) ||
                                 c.PersonCount.ToString().Contains(search));
    }

    // Get the total number of items after applying the search filter
    var totalItems = await query.CountAsync();
    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    CurrentPage = page;

    // Fetch the items for the current page with search applied
    ClassList = await query
        .OrderBy(c => c.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // Ensure the search term persists in the view
    ViewData["Search"] = search;

    return Page();
}

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

            return RedirectToPage("./Index");
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
                    return RedirectToPage("./Index");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        // GET: Export all classes as JSON
        public async Task<IActionResult> OnGetExportJsonAsync(string? columns)
        {
            var classData = await _context.Classes.ToListAsync();

            var selectedCols = columns?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var export = classData.Select(c => {
                var dict = new Dictionary<string, object>();
                if (selectedCols == null || selectedCols.Length == 0 || selectedCols.Contains("Id")) dict["Id"] = c.Id;
                if (selectedCols == null || selectedCols.Length == 0 || selectedCols.Contains("Name")) dict["Name"] = c.Name;
                if (selectedCols == null || selectedCols.Length == 0 || selectedCols.Contains("PersonCount")) dict["PersonCount"] = c.PersonCount;
                if (selectedCols == null || selectedCols.Length == 0 || selectedCols.Contains("Description")) dict["Description"] = c.Description;
                if (selectedCols == null || selectedCols.Length == 0 || selectedCols.Contains("IsActive")) dict["IsActive"] = c.IsActive;
                return dict;
            });

            var json = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
            return File(Encoding.UTF8.GetBytes(json), "application/json", "classes.json");
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