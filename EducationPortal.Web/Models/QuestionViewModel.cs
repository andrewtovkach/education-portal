using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class QuestionViewModel
    {
        public int QuestionNumber { get; set; }
        public Question Question { get; set; }
    }
}
