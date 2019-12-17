using System.Collections.Generic;
using EducationPortal.Web.Data.Enums;

namespace EducationPortal.Web.Data.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TrainingHours { get; set; }
        public CourseComplexity CourseComplexity { get; set; }

        public ICollection<EducationMaterial> EducationMaterials { get; set; }

        public ICollection<Test> Tests { get; set; }
    }
}
