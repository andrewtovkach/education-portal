using System.ComponentModel.DataAnnotations;

namespace EducationPortal.Web.Data.Enums
{
    public enum QuestionType
    {
        [Display(Name = "Один вариант ответа")]
        OneAnswer,
        [Display(Name = "Несколько вариантов ответа")]
        MultipleAnswers,
        [Display(Name = "Поле ввода")]
        TextInput
    }
}
