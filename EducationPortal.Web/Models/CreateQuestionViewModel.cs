using System.ComponentModel.DataAnnotations;
using EducationPortal.Web.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace EducationPortal.Web.Models
{
    public class CreateQuestionViewModel
    {
        [Required(ErrorMessage = "Поле Контент обязательно для заполнения")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Поле Тип вопроса обязательно для заполнения")]
        public QuestionType? QuestionType { get; set; }

        public IFormFile File { get; set; }
    }
}
