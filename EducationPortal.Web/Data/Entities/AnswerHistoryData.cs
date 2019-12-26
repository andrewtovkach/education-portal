using System;
using System.Collections.Generic;

namespace EducationPortal.Web.Data.Entities
{
    public class AnswerHistoryData
    {
        public int Id { get; set; }
        public int AttemptId { get; set; }
        public Attempt Attempt { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public DateTime Date { get; set; }
        public ICollection<AnswerHistory> AnswerHistories { get; set; }

        public AnswerHistoryData()
        {
            AnswerHistories = new List<AnswerHistory>();
        }
    }
}
