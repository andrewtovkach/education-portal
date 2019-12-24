using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Models
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Новый пароль обязателен для заполнения")]
        public string NewPassword { get; set; }
    }
}
