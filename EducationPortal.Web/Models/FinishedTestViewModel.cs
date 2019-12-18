using System.Collections.Generic;
using EducationPortal.Web.Data.Entities;

namespace EducationPortal.Web.Models
{
    public class FinishedTestViewModel
    {
        public string TestName { get; set; }
        public IEnumerable<Attempt> Attempts { get; set; }
    }
}
