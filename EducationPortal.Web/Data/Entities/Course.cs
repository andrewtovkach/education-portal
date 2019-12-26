using System.Collections.Generic;
using EducationPortal.Web.Data.Enums;

namespace EducationPortal.Web.Data.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TrainingHours { get; set; }
        public CourseComplexity CourseComplexity { get; set; }
        public ICollection<Module> Modules { get; set; }

        public Course()
        {
            Modules = new List<Module>();
        }

        public string ComplexityIconName
        {
            get
            {
                switch (CourseComplexity)
                {
                    case CourseComplexity.Beginner:
                        return "one.svg";
                    case CourseComplexity.Intermediate:
                        return "two.svg";
                    case CourseComplexity.Advanced:
                        return "three.svg";
                    default:
                        return "one.svg";
                }
            }
        }
    }
}
