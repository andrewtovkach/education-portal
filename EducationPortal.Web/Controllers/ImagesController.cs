using System.IO;
using System.Linq;
using EducationPortal.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EducationPortal.Web.Controllers
{
    public class ImagesController : Controller
    {
        private readonly EducationPortalDbContext _educationPortalDbContext;

        public ImagesController(EducationPortalDbContext educationPortalDbContext)
        {
            _educationPortalDbContext = educationPortalDbContext;
        }

        [HttpPost]
        public IActionResult UploadImage([FromForm]IFormFile file, int id)
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
                var question = _educationPortalDbContext.Questions.FirstOrDefault(x => x.Id == id);

                if (question == null)
                {
                    ModelState.AddModelError("id", "Id is incorrect");
                    return BadRequest();
                }

                question.Image = fileBytes;
                question.ImageContentType = file.ContentType;

                _educationPortalDbContext.SaveChanges();
            }

            return Ok();
        }
    }
}
