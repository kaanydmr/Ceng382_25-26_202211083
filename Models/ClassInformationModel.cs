using System.ComponentModel.DataAnnotations;


/*

Promt1:

can you update my model to include some validation and make it work with page model?

Promt2:
using System.ComponentModel.DataAnnotations;

namespace Week5.Models
{
public class ClassInformationModel
 {
public int Id { get; set; }
 [Required(ErrorMessage = "Class Name is required")]
 [StringLength(100, ErrorMessage = "Class Name cannot be longer than 100 characters")]
public string? ClassName { get; set; } = string.Empty;
 [Required(ErrorMessage = "Student Count is required")]
 [Range(1, int.MaxValue, ErrorMessage = "Student Count must be at least 1")]
public int? StudentCount { get; set; }
 [Required(ErrorMessage = "Description is required")]
 [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
public string? Description { get; set; } = string.Empty;
public ClassInformationModel()
 {
 }
public ClassInformationModel(string className, int studentCount, string description)
 {
ClassName = className;
StudentCount = studentCount;
Description = description;
 }
 }
}

this is my class information model and here is task This week, you will improve the table you created last week in your Razor Pages project. You
will add filtering and pagination features. However, filtering will be done on the data list in
the backend, not on the frontend.
The filtering logic must be written inside the OnGet methods.
You will also create a new model class called ClassInformationTable. This model will store
the filtered version of your main model and will be used to display data in the table. In this
model, the ID should not be shown in the table, but the ID will still be used in the
background for actions like edit, delete, or details.
In addition to filtering, you are required to implement pagination. To properly test the
pagination feature, you need to generate synthetic data. Make sure to create a list with at
least 100 sample records so you can see how the pagination works across multiple pages.
Tip :
When a filter value changes, the form should submit automatically or the user should click a
"Filter" button. This will trigger the OnGet method with the selected filter values passed as
query parameters.

*/
namespace Week5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        [StringLength(100, ErrorMessage = "Class Name cannot be longer than 100 characters")]
        public string? ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student Count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Student Count must be at least 1")]
        public int? StudentCount { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; } = string.Empty;

        public ClassInformationModel()
        {
        }

        public ClassInformationModel(string className, int studentCount, string description)
        {
            ClassName = className;
            StudentCount = studentCount;
            Description = description;
        }
    }
}