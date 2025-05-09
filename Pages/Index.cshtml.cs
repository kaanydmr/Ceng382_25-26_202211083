using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Week5.Models;
using Week5.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
/* 
Promt1:
This task will be implemented on the Index page (Index.cshtml and Index.cshtml.cs). • Create a folder named Models inside the project folder (note: folder name is plural). • Inside this folder, create a class named ClassInformationModel.cs. • This class will store the following properties: o Id (auto-incremented) o ClassName o StudentCount o Description The Id property will be automatically incremented each time a new item is added to the list. The list will act like a simple in-memory database. Make index.cshtml.cs

Promt2:
my page has a few additional functons here is my cshtml.cs Update the cshtml.cs to include my functions
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Models;
using System.Collections.Generic;

namespace Week5.Pages
{
public class IndexModel : PageModel
 {
 [BindProperty]
public ClassInformationModel ClassInfo { get; set; } = new();
public static List<ClassInformationModel> ClassInformationList { get; set; } = new();
 [BindProperty(SupportsGet = true)]
public int? EditId { get; set; }
public void OnGet(int? editId = null)
 {
EditId = editId;
if (EditId.HasValue)
 {
 // Prefill form with data for editing
var existingItem = ClassInformationList.Find(c => c.Id == EditId.Value);
if (existingItem != null)
 {
ClassInfo = new ClassInformationModel
 {
Id = existingItem.Id,
ClassName = existingItem.ClassName,
StudentCount = existingItem.StudentCount,
Description = existingItem.Description
 };
 }
 }
 }
public IActionResult OnPostAdd()
 {
if (!ModelState.IsValid)
return Page();
// Assign a unique ID
ClassInfo.Id = ClassInformationList.Count > 0
? ClassInformationList.Max(c => c.Id) + 1
: 1;
ClassInformationList.Add(ClassInfo);
return RedirectToPage();
 }
public IActionResult OnPostEdit()
 {
if (!ModelState.IsValid || !EditId.HasValue)
return Page();
var existingItem = ClassInformationList.Find(c => c.Id == EditId.Value);
if (existingItem != null)
 {
existingItem.ClassName = ClassInfo.ClassName;
existingItem.StudentCount = ClassInfo.StudentCount;
existingItem.Description = ClassInfo.Description;
 }
return RedirectToPage();
 }
public IActionResult OnPostDelete(int deleteId)
 {
ClassInformationList.RemoveAll(c => c.Id == deleteId);
return RedirectToPage();
 }
// Optional: Explicit Cancel method if you prefer
public IActionResult OnPostCancel()
 {
EditId = null;
ClassInfo = new ClassInformationModel();
return RedirectToPage();
 }
 }
}

    Promt3:
    Add a new method to export the data as JSON. This method should be called when the user clicks the "Export" button. The exported data should include only the selected columns and the search term (if any). The exported data should be in JSON format. You can use the JsonSerializer class from System.Text.Json namespace to serialize the data. The exported file should be named "ClassInformation.json".

*/

namespace Week5.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();
        
        public static List<ClassInformationModel> ClassInformationList { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public ClassInformationTable TableModel { get; set; } = new ClassInformationTable();
        
        // Available columns for selection
        public List<string> AvailableColumns { get; set; } = new List<string> 
        { 
            "ClassName", 
            "StudentCount", 
            "Description"
        };
        
        public void OnGet(int? editId = null, string[] selectedColumns = null)
        {
            // Generate sample data if list is empty (for testing pagination)
            if (ClassInformationList.Count == 0)
            {
                GenerateSampleData();
            }
            
            EditId = editId;
            if (EditId.HasValue)
            {
                // Prefill form with data for editing
                var existingItem = ClassInformationList.Find(c => c.Id == EditId.Value);
                if (existingItem != null)
                {
                    ClassInfo = new ClassInformationModel
                    {
                        Id = existingItem.Id,
                        ClassName = existingItem.ClassName,
                        StudentCount = existingItem.StudentCount,
                        Description = existingItem.Description
                    };
                }
            }
            
            // Store selected columns
            if (selectedColumns != null && selectedColumns.Length > 0)
            {
                TableModel.SelectedColumns = selectedColumns.ToList();
            }
            
            // Apply filtering and pagination
            ApplyFilteringAndPagination();
        }
        
        private void ApplyFilteringAndPagination()
        {
            // Start with all classes
            var filteredClasses = ClassInformationList.AsQueryable();
            
            // Apply universal search if term exists
            if (!string.IsNullOrWhiteSpace(TableModel.SearchTerm))
            {
                string searchTerm = TableModel.SearchTerm.ToLower();
                
                // Check if search term is numeric
                bool isNumeric = int.TryParse(searchTerm, out int numericValue);
                
                filteredClasses = filteredClasses.Where(c => 
                    (c.ClassName != null && c.ClassName.ToLower().Contains(searchTerm)) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm)) ||
                    (isNumeric && c.StudentCount == numericValue)
                );
            }
            
            // Store the total count before pagination
            TableModel.TotalItems = filteredClasses.Count();
            
            // Apply pagination
            TableModel.Classes = filteredClasses
                .Skip((TableModel.CurrentPage - 1) * TableModel.PageSize)
                .Take(TableModel.PageSize)
                .ToList();
        }
        
        public IActionResult OnPostAdd()
        {
            Console.WriteLine($"ClassInformationList size before adding: {ClassInformationList.Count}"); // Print size before adding

            if (!ModelState.IsValid)
            {
        
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }   
                
            // Assign a unique ID
            ClassInfo.Id = ClassInformationList.Count > 0
                ? ClassInformationList.Max(c => c.Id) + 1
                : 1;

            ClassInformationList.Add(ClassInfo);

            Console.WriteLine($"ClassInformationList size after adding: {ClassInformationList.Count}"); // Print size after adding
            
            return RedirectToPage();
        }
        
        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid || !EditId.HasValue)
                return Page();
                
            var existingItem = ClassInformationList.Find(c => c.Id == EditId.Value);
            if (existingItem != null)
            {
                existingItem.ClassName = ClassInfo.ClassName;
                existingItem.StudentCount = ClassInfo.StudentCount;
                existingItem.Description = ClassInfo.Description;
            }
            
            return RedirectToPage();
        }
        
        public IActionResult OnPostDelete(int deleteId)
        {
            ClassInformationList.RemoveAll(c => c.Id == deleteId);
            return RedirectToPage();
        }
        
        public IActionResult OnPostCancel()
        {
            EditId = null;
            ClassInfo = new ClassInformationModel();
            return RedirectToPage();
        }
        
        public IActionResult OnPostExport(string searchTerm, string[] selectedColumns)
        {
            var dataToExport = string.IsNullOrWhiteSpace(searchTerm) 
                ? ClassInformationList 
                : FilterData(ClassInformationList, searchTerm);
            
            var utils = Utils.Instance;
            var jsonData = utils.ExportToJson(dataToExport, selectedColumns?.ToList());
            
            return new ContentResult
            {
                Content = jsonData,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
        
        private IEnumerable<ClassInformationModel> FilterData(IEnumerable<ClassInformationModel> data, string searchTerm)
        {
            string term = searchTerm.ToLower();
            bool isNumeric = int.TryParse(term, out int numericValue);
            
            return data.Where(c => 
                (c.ClassName != null && c.ClassName.ToLower().Contains(term)) ||
                (c.Description != null && c.Description.ToLower().Contains(term)) ||
                (isNumeric && c.StudentCount == numericValue) ||
                (isNumeric && c.Id == numericValue)
            );
        }
        
        private void GenerateSampleData()
        {
            var random = new Random();
            string[] subjects = { "Math", "Science", "English", "History", "Computer Science", "Physics", "Chemistry", "Biology", "Art", "Music" };
            string[] levels = { 
                "Beginner", 
                "Elementary", 
                "Pre-Intermediate", 
                "Intermediate", 
                "Upper-Intermediate", 
                "Advanced", 
                "Proficient", 
                "Expert", 
                "Master", 
                "Distinguished" 
            };
            
            for (int i = 1; i <= 100; i++)
            {
                string subject = subjects[random.Next(subjects.Length)];
                string level = levels[random.Next(levels.Length)];
                int students = random.Next(10, 50);
                
                ClassInformationList.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"{subject} {level}",
                    StudentCount = students,
                    Description = $"This is a {level.ToLower()} {subject.ToLower()} class with {students} students."
                });
            }
        }
    }
}