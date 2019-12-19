using System.Collections.Generic;

namespace EducationPortal.Web.Data.Entities
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EducationMaterial> EducationMaterials { get; set; }
        public ICollection<Test> Tests { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
