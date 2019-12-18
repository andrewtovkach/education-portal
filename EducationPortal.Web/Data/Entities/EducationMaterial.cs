using EducationPortal.Web.Data.Enums;

namespace EducationPortal.Web.Data.Entities
{
    public class EducationMaterial
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public MaterialImportance MaterialImportance { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
