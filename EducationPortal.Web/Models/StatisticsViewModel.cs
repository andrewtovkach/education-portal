using System.Collections.Generic;

namespace EducationPortal.Web.Models
{
    public class StatisticsViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public IEnumerable<ChartValueViewModel> ChartValues { get; set; }
    }
}
