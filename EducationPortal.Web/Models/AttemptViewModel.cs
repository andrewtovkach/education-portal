using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class AttemptViewModel 
    {
        public Attempt Attempt { get; set; }
        public string TestName { get; set; }
        public int TestId { get; set; }
        public string CourseName { get; set; }
        public int CourseId { get; set; }
        public string ModuleName { get; set; }
        public int ModuleId { get; set; }
        public IEnumerable<Attempt> Attempts { get; set; }
        public int MaxNumberOfAttempts { get; set; }
    }
}
