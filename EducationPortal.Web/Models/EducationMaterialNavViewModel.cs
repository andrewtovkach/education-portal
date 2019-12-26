using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class EducationMaterialNavViewModel
    {
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public IEnumerable<EducationMaterial> EducationMaterials { get; set; }
    }
}
