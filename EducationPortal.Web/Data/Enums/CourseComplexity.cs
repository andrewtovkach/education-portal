using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Data.Enums
{
    public enum CourseComplexity
    {
        [Display(Name = "Продвинутый")]
        Advanced,
        [Display(Name = "Средний")]
        Intermediate,
        [Display(Name = "Начинающий")]
        Beginner
    }
}
