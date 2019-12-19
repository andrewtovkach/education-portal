using System.Collections.Generic;
namespace EducationPortal.Web.Models
{
    public class TestDetailsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; }
    }
}
