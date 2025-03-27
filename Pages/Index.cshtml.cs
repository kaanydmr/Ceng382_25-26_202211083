using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Models;
using System.Collections.Generic;

/*

 

Promt:
 

This task will be implemented on the Index page (Index.cshtml and Index.cshtml.cs). • Create a folder named Models inside the project folder (note: folder name is plural). • Inside this folder, create a class named ClassInformationModel.cs. • This class will store the following properties: o Id (auto-incremented) o ClassName o StudentCount o Description The Id property will be automatically incremented each time a new item is added to the list. The list will act like a simple in-memory database. Make index.cshtml.cs
 
Promt2:

can you fix the edit button to go back previous state after editing?
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