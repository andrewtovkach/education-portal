using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Поле Email не является валидным адресом электронной почты")]
        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        public string Email { get; set; }
    }
}
