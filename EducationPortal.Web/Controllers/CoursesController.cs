using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EducationPortal.Web.Data;
using EducationPortal.Web.Data.Entities;
using EducationPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spire.Doc;
using Spire.Xls;

namespace EducationPortal.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _educationPortalDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesController(ApplicationDbContext educationPortalDbContext,
            UserManager<IdentityUser> userManager)
        {
            _educationPortalDbContext = educationPortalDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (!User.IsInRole("tutor"))
                return View(_educationPortalDbContext.Courses);

            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;
            return View(_educationPortalDbContext.Courses.Where(x => x.CreatedBy == Guid.Parse(userId)));

        }

        public IActionResult Details(int id, int? moduleId)
        {
            var course = _educationPortalDbContext.Courses.Where(c => c.Id == id)
                .Include(c => c.Modules)
                .ThenInclude(c => c.EducationMaterials)
                .Include(c => c.Modules)
                .ThenInclude(c => c.Tests)
                .ThenInclude(x => x.Questions)
                .FirstOrDefault();

            if (course == null)
            {
                return NotFound();
            }

            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;
            var courses = User.IsInRole("tutor")
                ? _educationPortalDbContext.Courses.Where(x => x.CreatedBy == Guid.Parse(userId)).ToList()
                : _educationPortalDbContext.Courses.ToList();

            if (!moduleId.HasValue)
            {
                var firstModule = course.Modules.FirstOrDefault();

                if (firstModule == null)
                {
                    return View(new CourseDetailsViewModel
                    {
                        Courses = courses,
                        CourseId = course.Id,
                        CourseName = course.Name,
                        Modules = new List<Module>(),
                        EducationMaterials = new List<EducationMaterial>(),
                        Tests = new List<TestViewModel>()
                    });
                }

                moduleId = firstModule.Id;
            }

            var currentModule = course.Modules.FirstOrDefault(x => x.Id == moduleId);

            if (currentModule == null)
            {
                return NotFound();
            }

            var testsCollection = GetTestViewModels(currentModule);

            var courseDetailsViewModel = new CourseDetailsViewModel
            {
                Courses = courses,
                CourseId = course.Id,
                CourseName = course.Name,
                Modules = course.Modules,
                Tests = testsCollection,
                EducationMaterials = currentModule.EducationMaterials,
                ActiveModuleId = moduleId.Value
            };

            return View(courseDetailsViewModel);
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult Create()
        {
            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;

            ViewBag.Courses = User.IsInRole("tutor")
                ? _educationPortalDbContext.Courses.Where(x => x.CreatedBy == Guid.Parse(userId)).ToList()
                : _educationPortalDbContext.Courses.ToList();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public IActionResult Create(CreateCourseViewModel model)
        {
            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;

            ViewBag.Courses = User.IsInRole("tutor")
                ? _educationPortalDbContext.Courses.Where(x => x.CreatedBy == Guid.Parse(userId)).ToList()
                : _educationPortalDbContext.Courses.ToList();

            if (!ModelState.IsValid)
                return View(model);

            var course = new Course
            {
                Name = model.Name,
                CourseComplexity = model.CourseComplexity.Value,
                TrainingHours = model.TrainingHours.Value,
                CreatedBy = Guid.Parse(userId)
            };

            var courseEntity = _educationPortalDbContext.Courses.Add(course);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id = courseEntity.Entity.Id });
        }

        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public ActionResult Delete(int id)
        {
            var course = _educationPortalDbContext.Courses.FirstOrDefault(x => x.Id == id);
            if (course == null)
                return RedirectToAction("Index");

            _educationPortalDbContext.Courses.Remove(course);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult CreateModule(int id)
        {
            var course = _educationPortalDbContext.Courses.Include(x => x.Modules)
                .FirstOrDefault(x => x.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = course.Id;
            ViewBag.CourseName = course.Name;
            ViewBag.Modules = course.Modules; 

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public IActionResult CreateModule(CreateModuleViewModel model, int id)
        {
            var course = _educationPortalDbContext.Courses.FirstOrDefault(x => x.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = course.Id;
            ViewBag.CourseName = course.Name;
            ViewBag.Modules = course.Modules;

            if (!ModelState.IsValid)
                return View(model);

            var module = new Module
            {
                Name = model.Name,
                CourseId = id
            };

            course.Modules.Add(module);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id, moduleId = module.Id });
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult DeleteModule(int id, int courseId)
        {
            var module = _educationPortalDbContext.Modules.FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            _educationPortalDbContext.Modules.Remove(module);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id = courseId });
        }

        public IActionResult EducationMaterial(int id)
        {
            var educationMaterial = _educationPortalDbContext.EducationMaterials.FirstOrDefault(x => x.Id == id);

            var module = _educationPortalDbContext.Modules.Include(x => x.Course)
                .Include(x => x.EducationMaterials)
                .FirstOrDefault(x => x.Id == educationMaterial.ModuleId);

            if (module == null)
            {
                return NotFound();
            }

            if (educationMaterial == null)
            {
                return NotFound();
            }

            var educationMaterialViewModel = new EducationMaterialViewModel
            {
                Id = educationMaterial.Id,
                Name = educationMaterial.Name
            };

            ViewBag.CourseId = module.CourseId;
            ViewBag.CourseName = module.Course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.EducationMaterials = module.EducationMaterials;

            return View(educationMaterialViewModel);
        }

        public IActionResult EducationFileContent(int id)
        {
            var educationMaterial = _educationPortalDbContext.EducationMaterials.FirstOrDefault(x => x.Id == id);

            if (educationMaterial == null)
            {
                return NotFound();
            }

            var temporaryFolder = GetTemporaryFolderPath();

            var randomName = DateTime.Now.Ticks.ToString();
            switch (educationMaterial.ContentType)
            {
                case EducationMaterialContentType.Docx:
                {
                    return GetWordFileContext(temporaryFolder, randomName, educationMaterial.Data);
                }
                case EducationMaterialContentType.Doc:
                {
                    return GetWordFileContext(temporaryFolder, randomName, educationMaterial.Data, "doc");
                }
                case EducationMaterialContentType.Xlsx:
                {
                    return GetExcelFileContent(temporaryFolder, randomName, educationMaterial.Data);
                }
                case EducationMaterialContentType.Xls:
                {
                    return GetExcelFileContent(temporaryFolder, randomName, educationMaterial.Data, "xls");
                }
                default:
                {
                    return new FileContentResult(educationMaterial.Data, educationMaterial.ContentType);
                }
            }
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddEducationMaterial(int id)
        {
            var module = _educationPortalDbContext.Modules
                .Include(x => x.Course)
                .Include(x => x.EducationMaterials)
                .FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = module.CourseId;
            ViewBag.CourseName = module.Course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.EducationMaterials = module.EducationMaterials;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddEducationMaterial(CreateEducationMaterialViewModel model, int id)
        {
            var module = _educationPortalDbContext.Modules
                .Include(x => x.Course)
                .FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = module.CourseId;
            ViewBag.CourseName = module.Course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.EducationMaterials = module.EducationMaterials;

            if (!ModelState.IsValid)
                return View(model);

            UploadFileToDb(model, module);

            return RedirectToAction("Details", new { id = module.CourseId, moduleId = module.Id });
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult DeleteEducationMaterial(int id, int moduleId, int courseId)
        {
            var educationMaterial = _educationPortalDbContext.EducationMaterials.FirstOrDefault(x => x.Id == id);

            if (educationMaterial == null)
            {
                return RedirectToAction("Details", new { id = courseId, moduleId });
            }

            _educationPortalDbContext.EducationMaterials.Remove(educationMaterial);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id = courseId, moduleId });
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddTest(int id)
        {
            var module = _educationPortalDbContext.Modules.Include(x => x.Course).FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = module.CourseId;
            ViewBag.CourseName = module.Course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == module.Id);

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddTest(CreateTestViewModel model, int id)
        {
            var module = _educationPortalDbContext.Modules.Include(x => x.Course).FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = module.CourseId;
            ViewBag.CourseName = module.Course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == module.Id);

            if (!ModelState.IsValid)
                return View(model);

            var test = new Test
            {
                MaxNumberOfAttempts = model.MaxNumberOfAttempts.Value,
                TimeLimit = model.TimeLimit.Value,
                Name = model.Name
            };

            module.Tests.Add(test);

            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Info",  "Tests", new { id = test.Id });
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult DeleteTest(int id, int courseId, int moduleId)
        {
            var test = _educationPortalDbContext.Tests.FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            _educationPortalDbContext.Tests.Remove(test);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id = courseId, moduleId });
        }

        #region Private Methods
        private IEnumerable<TestViewModel> GetTestViewModels(Module currentModule)
        {
            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;
            var testsCollection = new List<TestViewModel>();

            foreach (var test in currentModule.Tests)
            {
                var testCompletion = _educationPortalDbContext.TestCompletions.Where(x => x.UserId == Guid.Parse(userId) && x.TestId == test.Id)
                    .Include(x => x.Attempts)
                    .FirstOrDefault();

                var hasTestCompletions = testCompletion != null;
                var testViewModel = new TestViewModel { HasTestCompletions = hasTestCompletions, Test = test };

                if (hasTestCompletions)
                {
                    testViewModel.AttemptsCount = testCompletion.Attempts.Count;
                    testViewModel.AverageScore = testCompletion.Attempts.Average(x => x.Score);
                }

                testsCollection.Add(testViewModel);
            }

            return testsCollection;
        }

        private void UploadFileToDb(CreateEducationMaterialViewModel model, Module module)
        {
            using (var ms = new MemoryStream())
            {
                model.File.CopyTo(ms);
                var fileBytes = ms.ToArray();

                var educationMaterial = new EducationMaterial
                {
                    Data = fileBytes,
                    ContentType = model.File.ContentType,
                    MaterialImportance = model.MaterialImportance.Value,
                    Name = model.Name
                };

                module.EducationMaterials.Add(educationMaterial);
                _educationPortalDbContext.SaveChanges();
            }
        }

        private static void WriteBytesToFile(string filePath, byte[] data)
        {
            using (Stream file = System.IO.File.OpenWrite(filePath))
            {
                file.Write(data, 0, data.Length);
                file.Close();
            }
        }

        private static void SaveXlsxFileToHtml(string xlsFilePath, string htmlFilePath)
        {
            var workbook = new Workbook();
            workbook.LoadFromFile(xlsFilePath);

            var sheet = workbook.Worksheets[0];
            sheet.SaveToHtml(htmlFilePath);
        }

        private static void SaveDocxFileToHtml(string docFilePath, string htmlFilePath)
        {
            var document = new Document();
            document.LoadFromFile(docFilePath);
            document.SaveToFile(htmlFilePath, Spire.Doc.FileFormat.Html);
        }

        private IActionResult GetWordFileContext(string temporaryFolder, string randomName,
            byte [] data, string extension = "docx")
        {
            var htmlFilePath = Path.Combine(temporaryFolder, randomName + ".html");
            var docFilePath = Path.Combine(temporaryFolder, randomName + "." + extension);
            WriteBytesToFile(docFilePath, data);
            SaveDocxFileToHtml(docFilePath, htmlFilePath);

            return File(System.IO.File.OpenRead(htmlFilePath), "text/html");
        }

        private IActionResult GetExcelFileContent(string temporaryFolder, string randomName,
            byte[] data, string extension = "xlsx")
        {
            var htmlFilePath = Path.Combine(temporaryFolder, randomName + ".html");
            var xlsFilePath = Path.Combine(Path.GetTempPath(), randomName + "." + extension);
            WriteBytesToFile(xlsFilePath, data);
            SaveXlsxFileToHtml(xlsFilePath, htmlFilePath);

            return File(System.IO.File.OpenRead(htmlFilePath), "text/html");
        }

        private static string GetTemporaryFolderPath()
        {
            var temporaryFolder = Path.Combine(Path.GetTempPath(), "EducationPortal");
            if (!Directory.Exists(temporaryFolder))
            {
                Directory.CreateDirectory(temporaryFolder);
            }

            foreach (var file in new DirectoryInfo(temporaryFolder).GetFiles())
            {
                file.Delete();
            }

            return temporaryFolder;
        }
        #endregion
    }
}
