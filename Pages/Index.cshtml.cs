using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Models;
using System.Collections.Generic;

/*
Promt:
This task will be implemented on the Index page (Index.cshtml and Index.cshtml.cs). • Create a folder named Models inside the project folder (note: folder name is plural). • Inside this folder, create a class named ClassInformationModel.cs. • This class will store the following properties: o Id (auto-incremented) o ClassName o StudentCount o Description The Id property will be automatically incremented each time a new item is added to the list. The list will act like a simple in-memory database. Make index.cshtml.cs
*/

namespace Week5.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        public static List<ClassInformationModel> ClassInformationList { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; } // Used to determine editing state

        public void OnGet(int? editId = null)
        {
            EditId = editId;

            if (EditId.HasValue)
            {
                // Prefill form with data for editing
                ClassInfo = ClassInformationList.Find(c => c.Id == EditId.Value) ?? new();
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            ClassInformationList.Add(ClassInfo); // Add new item
            return RedirectToPage(); // Refresh the page
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

            return RedirectToPage(); // Refresh the page
        }

        public IActionResult OnPostDelete(int deleteId)
        {
            ClassInformationList.RemoveAll(c => c.Id == deleteId); // Delete item
            return RedirectToPage(); // Refresh the page
        }

        public IActionResult OnPostCancel()
        {
            EditId = null; // Clear the EditId to exit edit mode
            ClassInfo = new ClassInformationModel(); // Reset the form fields for new submissions
            return RedirectToPage(); // Refresh the page to reflect changes
        }


    }
}