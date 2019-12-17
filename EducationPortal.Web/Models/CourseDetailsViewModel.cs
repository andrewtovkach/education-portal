using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class CourseDetailsViewModel
    {
        public string CourseName { get; set; }
        public IEnumerable<EducationMaterial> EducationMaterials { get; set; }
        public IEnumerable<Test> Tests { get; set; }
    }
}
