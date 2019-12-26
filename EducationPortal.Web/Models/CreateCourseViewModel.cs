using System.ComponentModel.DataAnnotations;
using EducationPortal.Web.Data.Enums;

namespace EducationPortal.Web.Models
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Пожалуйста введите валидное количество учебных часов")]
        [Required(ErrorMessage = "Поле Количество учебных часов обязательно для заполнения")]
        public int? TrainingHours { get; set; }

        [Required(ErrorMessage = "Поле Сложность курса обязательно для заполнения")]
        public CourseComplexity? CourseComplexity { get; set; }
    }
}
