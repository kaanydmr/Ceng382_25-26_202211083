using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Week5.Models
{
    public class ClassInformationTable
    {
        public List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
        
        // Single search filter
        public string? SearchTerm { get; set; }
        
        // Selected columns for export
        public List<string> SelectedColumns { get; set; } = new List<string>();
        
        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    }
}