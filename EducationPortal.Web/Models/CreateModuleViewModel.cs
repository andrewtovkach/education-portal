using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Models
{
    public class CreateModuleViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        public string Name { get; set; }
    }
}
