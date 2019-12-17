namespace EducationPortal.Web.Data.Entities
{
    public class AnswerHistory
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
        public bool IsCorrect { get; set; }
        public int NumberOfPoints { get; set; }
        public int AnswerHistoryDataId { get; set; }
        public AnswerHistoryData AnswerHistoryData { get; set; }
        public string TextInput { get; set; }
    }
}
