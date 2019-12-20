using System;
using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class TestDetailsViewModel
    {
        public const string OneAnswerTitle = "Допустим только один вариант ответа";
        public const string MultipleAnswersTitle = "Допустимо несколько вариантов ответа";

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public DateTime TimeToFinish { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; }
        public IEnumerable<Test> Tests { get; set; }
        public IEnumerable<EducationMaterial> EducationMaterials { get; set; }
    }
}
