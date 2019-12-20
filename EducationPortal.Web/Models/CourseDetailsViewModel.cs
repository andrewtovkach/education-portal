using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class CourseDetailsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public IEnumerable<Module> Modules { get; set; }
        public IEnumerable<EducationMaterial> EducationMaterials { get; set; }
        public IEnumerable<TestViewModel> Tests { get; set; }
        public int ActiveModuleId { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
