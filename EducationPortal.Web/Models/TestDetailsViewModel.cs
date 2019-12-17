using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class TestDetailsViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public IEnumerable<Question> Questions { get; set; }
    }
}
