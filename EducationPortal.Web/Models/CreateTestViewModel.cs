using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Models
{
    public class CreateTestViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле Время на тест обязательно для заполнения")]
        [Range(1, 180, ErrorMessage = "Значение должно быть в диапазоне от 1 до 180 минут")]
        public int? TimeLimit { get; set; }

        [Required(ErrorMessage = "Поле Макс количество попыток обязательно для заполнения")]
        [Range(1, 100, ErrorMessage = "Значение должно быть в диапазоне от 1 до 100")]
        public int? MaxNumberOfAttempts { get; set; }
    }
}
