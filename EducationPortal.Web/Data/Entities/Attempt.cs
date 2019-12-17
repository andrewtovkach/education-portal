namespace EducationPortal.Web.Data.Entities
{
    public class Attempt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public TestCompletion TestCompletion { get; set; }
        public int TestCompletionId { get; set; }
    }
}
