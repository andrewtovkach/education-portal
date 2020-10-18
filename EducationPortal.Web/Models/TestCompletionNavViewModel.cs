using System.Collections.Generic;

namespace EducationPortal.Web.Models
{
    public class TestCompletionNavViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public IEnumerable<TestCompeletionViewModel> TestCompeletions { get; set; }
    }
}
