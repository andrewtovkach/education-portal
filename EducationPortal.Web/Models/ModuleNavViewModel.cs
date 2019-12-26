using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class ModuleNavViewModel
    {
        public IEnumerable<Module> Modules { get; set; }
        public int CourseId { get; set; }
    }
}
