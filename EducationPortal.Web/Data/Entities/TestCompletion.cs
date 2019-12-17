using System.Collections.Generic;

namespace EducationPortal.Web.Data.Entities
{
    public class TestCompletion
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }

        public ICollection<Attempt> Attempts { get; set; }
    }
}
