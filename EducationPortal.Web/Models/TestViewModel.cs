using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class TestViewModel 
    {
        public Test Test { get; set; }
        public bool HasTestCompletions { get; set; }
        public int AttemptsCount { get; set; }
        public double? AverageScore { get; set; }
    }
}
