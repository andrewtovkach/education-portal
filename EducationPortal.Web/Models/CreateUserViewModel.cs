using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Models
{
    public class CreateUserViewModel
    {
        [EmailAddress(ErrorMessage = "Поле Email не является валидным адресом электронной почты")]
        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        public string Password { get; set; }
    }
}
