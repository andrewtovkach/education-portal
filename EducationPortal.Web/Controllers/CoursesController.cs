using System;
using System.Collections.Generic;
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
                    return NotFound();
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
                CourseId = course.Id,
                CourseName = course.Name,
                Modules = course.Modules,
                Tests = testsCollection,
                EducationMaterials = currentModule.EducationMaterials,
                ActiveModuleId = moduleId.Value
            };

            return View(courseDetailsViewModel);
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
        #endregion
    }
}
