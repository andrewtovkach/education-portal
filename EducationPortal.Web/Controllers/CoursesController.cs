﻿using System.Linq;
using EducationPortal.Web.Data;
using EducationPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationPortal.Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly EducationPortalDbContext _educationPortalDbContext;

        public CoursesController(EducationPortalDbContext educationPortalDbContext)
        {
            _educationPortalDbContext = educationPortalDbContext;
        }

        public IActionResult Index()
        {
            return View(_educationPortalDbContext.Courses);
        }

        public IActionResult Details(int? id)
        {
            var course = _educationPortalDbContext.Courses.Where(c => c.Id == id)
                .Include(c => c.EducationMaterials)
                .Include(c => c.Tests)
                .FirstOrDefault();

            var courseDetailsViewModel = new CourseDetailsViewModel
            {
                CourseName = course.Name,
                EducationMaterials = course.EducationMaterials,
                Tests = course.Tests
            };

            return View(courseDetailsViewModel);
        }

        public IActionResult GetEducationFileContent(int? id)
        {
            var educationMaterial = _educationPortalDbContext.EducationMaterials.FirstOrDefault(x => x.Id == id);
            return new FileContentResult(educationMaterial.Data, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
