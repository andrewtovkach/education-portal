using System.ComponentModel.DataAnnotations;
using EducationPortal.Web.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace EducationPortal.Web.Models
{
    public class CreateEducationMaterialViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле Важность обязательно для заполнения")]
        public MaterialImportance? MaterialImportance { get; set; }

        [Required(ErrorMessage = "Пожалуйста выберите файл")]
        public IFormFile File { get; set; }
    }
}
