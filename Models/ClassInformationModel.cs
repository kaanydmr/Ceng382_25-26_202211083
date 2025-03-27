using System.ComponentModel.DataAnnotations;


/*

Promt:

can you update my model to include some validation and make it work with page model?


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