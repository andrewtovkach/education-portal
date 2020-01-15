using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class CreateTestDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Question> Questions { get; set; }
    }
}
