using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Week5.Models;
using Week5.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


*/

namespace Week5.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();
        
        public static List<ClassInformationModel> ClassInformationList { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public ClassInformationTable TableModel { get; set; } = new ClassInformationTable();
        
        public void OnGet(int? editId = null)
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
            
            // Apply filtering and pagination
            ApplyFilteringAndPagination();
        }
        
        private void ApplyFilteringAndPagination()
        {
            // Start with all classes
            var filteredClasses = ClassInformationList.AsQueryable();
            
            // Apply filters if they exist
            if (!string.IsNullOrWhiteSpace(TableModel.FilterClassName))
            {
                filteredClasses = filteredClasses.Where(c => c.ClassName != null && 
                    c.ClassName.Contains(TableModel.FilterClassName, StringComparison.OrdinalIgnoreCase));
            }
            
            if (TableModel.FilterMinStudentCount.HasValue)
            {
                filteredClasses = filteredClasses.Where(c => c.StudentCount >= TableModel.FilterMinStudentCount);
            }
            
            if (TableModel.FilterMaxStudentCount.HasValue)
            {
                filteredClasses = filteredClasses.Where(c => c.StudentCount <= TableModel.FilterMaxStudentCount);
            }
            
            if (!string.IsNullOrWhiteSpace(TableModel.FilterDescription))
            {
                filteredClasses = filteredClasses.Where(c => c.Description != null && 
                    c.Description.Contains(TableModel.FilterDescription, StringComparison.OrdinalIgnoreCase));
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
        
        public IActionResult OnPostCancel()
        {
            EditId = null;
            ClassInfo = new ClassInformationModel();
            return RedirectToPage();
        }
        
        public IActionResult OnPostFilter()
        {
            // Just redirect to get with the filter parameters
            return RedirectToPage();
        }
        
        public IActionResult OnPostExportJson()
        {
            // Export the entire list to JSON using the utility class
            string jsonData = Utils.Instance.ExportToJson(ClassInformationList);

            // Return JSON as a downloadable file
            return File(System.Text.Encoding.UTF8.GetBytes(jsonData), "application/json", "Classes.json");
        }


        public IActionResult OnPostExportFilteredJson(string selectedColumns)
        {
            // Parse selected columns
            var columns = selectedColumns?.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            // Check if any columns were selected; if not, include all by default
            if (columns == null || !columns.Any())
            {
                columns = new List<string> { "ClassName", "StudentCount", "Description" };
            }

            // Use JsonExportUtils to generate JSON based on the selected columns
            string jsonFilteredData = Utils.Instance.ExportToJson(TableModel.Classes, columns);

            // Return JSON as a downloadable file
            return File(System.Text.Encoding.UTF8.GetBytes(jsonFilteredData), "application/json", "FilteredClasses.json");
        }
        


        private void GenerateSampleData()
        {
            var random = new Random();
            string[] subjects = { "Math", "Science", "English", "History", "Computer Science", "Physics", "Chemistry", "Biology", "Art", "Music" };
            string[] levels = {"Beginner", "Elementary", "Pre-Intermediate", "Intermediate", "Upper-Intermediate",  "Advanced", "Proficient", "Expert", "Master", "Doctorate"};
            
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