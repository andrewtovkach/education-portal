using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Data.Enums
{
    public enum MaterialImportance
    {
        [Display(Name = "Высокий")]
        High,
        [Display(Name = "Средний")]
        Normal,
        [Display(Name = "Низкий")]
        Low
    }
}
