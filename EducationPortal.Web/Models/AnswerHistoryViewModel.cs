namespace EducationPortal.Web.Models
{
    public class AnswerHistoryViewModel
    {
        public bool IsCorrect { get; set; }
        public int NumberOfPoints { get; set; }
        public string Content { get; set; }
        public int QuestionId { get; set; }
    }
}
