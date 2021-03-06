﻿using EducationPortal.Web.Data.Entities;
using System.Collections.Generic;

namespace EducationPortal.Web.Models
{
    public class QuestionDetailsViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }

        public int TestId { get; set; }

        public IEnumerable<Answer> Answers { get; set; }
    }
}
