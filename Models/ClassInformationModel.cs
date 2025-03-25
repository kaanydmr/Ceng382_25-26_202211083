namespace Week5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        public string? ClassName { get; set; } = string.Empty; 
        public int? StudentCount { get; set; }
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
