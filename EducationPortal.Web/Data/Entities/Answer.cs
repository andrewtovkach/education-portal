﻿namespace EducationPortal.Web.Data.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public int NumberOfPoints { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
