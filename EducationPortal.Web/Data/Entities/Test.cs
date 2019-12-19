
using System.Collections.Generic;

namespace EducationPortal.Web.Data.Entities
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TimeLimit { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
