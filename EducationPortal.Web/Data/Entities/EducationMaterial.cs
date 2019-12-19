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
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public string ImportanceIconName
        {
            get
            {
                switch (MaterialImportance)
                {
                    case MaterialImportance.High:
                        return "imp_one.svg";
                    case MaterialImportance.Normal:
                        return "imp_two.svg";
                    case MaterialImportance.Low:
                        return "imp_three.svg";
                    default:
                        return "imp_one.svg";
                }
            }
        }
    }
}
