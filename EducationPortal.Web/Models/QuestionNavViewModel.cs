using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class QuestionNavViewModel
    {
        public int TestId { get; set; }
        public IEnumerable<Question> Questions { get; set; }
    }
}
