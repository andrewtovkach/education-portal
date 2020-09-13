using EducationPortal.Web.Data.Entities;
using System.Collections.Generic;

namespace EducationPortal.Web.Models
{
    public class AnswerNavViewModel
    {
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
    }
}
