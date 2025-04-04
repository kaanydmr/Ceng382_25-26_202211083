using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Week5.Models
{
    public class ClassInformationTable
    {
        public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
        
        // Filter properties
        public string? FilterClassName { get; set; }
        public int? FilterMinStudentCount { get; set; }
        public int? FilterMaxStudentCount { get; set; }
        public string? FilterDescription { get; set; }
        
        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    }
}