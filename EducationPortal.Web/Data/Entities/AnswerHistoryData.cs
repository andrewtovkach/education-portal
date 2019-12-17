using System;
using System.Collections.Generic;

namespace EducationPortal.Web.Data.Entities
{
    public class AnswerHistoryData
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public ICollection<AnswerHistory> AnswerHistories { get; set; }
        public string TextInput { get; set; }
    }
}
