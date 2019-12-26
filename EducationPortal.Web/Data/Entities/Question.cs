using System;
using System.Collections.Generic;
using EducationPortal.Web.Data.Enums;

namespace EducationPortal.Web.Data.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public QuestionType QuestionType { get; set; }
        public byte [] Image { get; set; }
        public string ImageContentType { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
        public ICollection<Answer> Answers { get; set; }

        public Question()
        {
            Answers = new List<Answer>();
        }

        public string ImageSrc
        {
            get
            {
                var imageBase64 = Convert.ToBase64String(Image);
                return $"data:image/gif;base64,{imageBase64}";
            }
        }
    }
}
