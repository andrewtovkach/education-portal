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

namespace EducationPortal.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly EducationPortalDbContext _educationPortalDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesController(EducationPortalDbContext educationPortalDbContext,
            UserManager<IdentityUser> userManager)
        {
            _educationPortalDbContext = educationPortalDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_educationPortalDbContext.Courses);
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

            if (!moduleId.HasValue)
            {
                var firstModule = course.Modules.FirstOrDefault();

                if (firstModule == null)
                {
                    return View(new CourseDetailsViewModel
                    {
                        Courses = _educationPortalDbContext.Courses,
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
                Courses = _educationPortalDbContext.Courses,
                CourseId = course.Id,
                CourseName = course.Name,
                Modules = course.Modules,
                Tests = testsCollection,
                EducationMaterials = currentModule.EducationMaterials,
                ActiveModuleId = moduleId.Value
            };

            return View(courseDetailsViewModel);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.Courses = _educationPortalDbContext.Courses;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Create(CreateCourseViewModel model)
        {
            ViewBag.Courses = _educationPortalDbContext.Courses;

            if (!ModelState.IsValid)
                return View(model);

            var course = new Course
            {
                Name = model.Name,
                CourseComplexity = model.CourseComplexity.Value,
                TrainingHours = model.TrainingHours.Value
            };

            var courseEntity = _educationPortalDbContext.Courses.Add(course);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id = courseEntity.Entity.Id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            var course = _educationPortalDbContext.Courses.FirstOrDefault(x => x.Id == id);
            if (course == null)
                return RedirectToAction("Index");

            var moduleIds = _educationPortalDbContext.Modules.Where(x => x.CourseId == id).Select(x => x.Id);
            var tests = _educationPortalDbContext.Tests.Where(x => moduleIds.Contains(x.ModuleId));

            _educationPortalDbContext.Tests.RemoveRange(tests);
            _educationPortalDbContext.Courses.Remove(course);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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

        public IActionResult EducationFileContent(int id)
        {
            var educationMaterial = _educationPortalDbContext.EducationMaterials.FirstOrDefault(x => x.Id == id);

            if (educationMaterial == null)
            {
                return NotFound();
            }

            return new FileContentResult(educationMaterial.Data, educationMaterial.ContentType);
        }

        public IActionResult AddEducationMaterial(int id)
        {
            var module = _educationPortalDbContext.Modules.Include(x => x.EducationMaterials)
                .FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            var course = _educationPortalDbContext.Courses.FirstOrDefault(x => x.Id == module.CourseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = course.Id;
            ViewBag.CourseName = course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.EducationMaterials = module.EducationMaterials;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult AddEducationMaterial(CreateEducationMaterialViewModel model, int id)
        {
            var module = _educationPortalDbContext.Modules.FirstOrDefault(x => x.Id == id);

            if (module == null)
            {
                return NotFound();
            }

            var course = _educationPortalDbContext.Courses.FirstOrDefault(x => x.Id == module.CourseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = course.Id;
            ViewBag.CourseName = course.Name;
            ViewBag.ModuleId = module.Id;
            ViewBag.ModuleName = module.Name;
            ViewBag.EducationMaterials = module.EducationMaterials;

            if (!ModelState.IsValid)
                return View(model);

            UploadFileToDb(model, module);

            return RedirectToAction("Details", new { id = course.Id, moduleId = module.Id });
        }

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
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

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
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

            if (!ModelState.IsValid)
                return View(model);

            module.Tests.Add(new Test
            {
                MaxNumberOfAttempts = model.MaxNumberOfAttempts.Value,
                TimeLimit = model.TimeLimit.Value,
                Name = model.Name
            });

            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", new { id = module.CourseId, moduleId = id });
        }

        [Authorize(Roles = "admin")]
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
        #endregion
    }
}
