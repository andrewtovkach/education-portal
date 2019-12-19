using System.IO;
using System.Linq;
using EducationPortal.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EducationPortal.Web.Controllers
{
    public class EducationMaterialsController : Controller
    {
        private readonly EducationPortalDbContext _educationPortalDbContext;

        public EducationMaterialsController(EducationPortalDbContext educationPortalDbContext)
        {
            _educationPortalDbContext = educationPortalDbContext;
        }

        [HttpPost]
        public IActionResult UploadEducationMaterial([FromForm]IFormFile file, int id)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "The content of the file is empty");
                return BadRequest(ModelState);
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                var educationMaterial = _educationPortalDbContext.EducationMaterials.FirstOrDefault(x => x.Id == id);

                if (educationMaterial == null)
                {
                    return NotFound();
                }

                educationMaterial.Data = fileBytes;
                educationMaterial.ContentType = file.ContentType;

                _educationPortalDbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
