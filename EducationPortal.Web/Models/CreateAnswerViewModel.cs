using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Models
{
    public class CreateAnswerViewModel
    {
        [Required(ErrorMessage = "Поле Контент обязательно для заполнения")]
        public string Content { get; set; }

        public bool IsCorrect { get; set; }
    }
}
