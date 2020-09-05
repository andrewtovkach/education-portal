using EducationPortal.Web.Data;
using EducationPortal.Web.Data.Entities;
using EducationPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EducationPortal.Web.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _educationPortalDbContext;

        public QuestionsController(ApplicationDbContext educationPortalDbContext,
            UserManager<IdentityUser> userManager)
        {
            _educationPortalDbContext = educationPortalDbContext;
            _userManager = userManager;
        }

        public IActionResult Details(int id)
        {
            var question = _educationPortalDbContext.Questions.Where(x => x.Id == id)
                .Include(x => x.Test)
                .ThenInclude(x => x.Module)
                .ThenInclude(x => x.Course)
                .Include(x => x.Answers)
                .FirstOrDefault();

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = question.Test.Module.CourseId;
            ViewBag.CourseName = question.Test.Module.Course.Name;
            ViewBag.ModuleId = question.Test.ModuleId;
            ViewBag.ModuleName = question.Test.Module.Name;
            ViewBag.TestId = question.Test.Id;
            ViewBag.TestName = question.Test.Name;

            var questionDetailsViewModel = new QuestionDetailsViewModel
            {
                QuestionId = question.Id,
                QuestionContent = question.Content,
                Answers = question.Answers
            };

            return View(questionDetailsViewModel);
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddAnswer(int id)
        {
            var question = _educationPortalDbContext.Questions
                .Include(x => x.Test)
                .ThenInclude(x => x.Module)
                .ThenInclude(x => x.Course)
                .Include(x => x.Answers)
                .FirstOrDefault(x => x.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = question.Test.Module.CourseId;
            ViewBag.CourseName = question.Test.Module.Course.Name;
            ViewBag.ModuleId = question.Test.ModuleId;
            ViewBag.ModuleName = question.Test.Module.Name;
            ViewBag.TestId = question.Test.Id;
            ViewBag.TestName = question.Test.Name;

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddAnswer(CreateAnswerViewModel model, int id)
        {
            var question = _educationPortalDbContext.Questions
                .Include(x => x.Test)
                .ThenInclude(x => x.Module)
                .ThenInclude(x => x.Course)
                .Include(x => x.Answers)
                .FirstOrDefault(x => x.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = question.Test.Module.CourseId;
            ViewBag.CourseName = question.Test.Module.Course.Name;
            ViewBag.ModuleId = question.Test.ModuleId;
            ViewBag.ModuleName = question.Test.Module.Name;
            ViewBag.TestId = question.Test.Id;
            ViewBag.TestName = question.Test.Name;

            if (!ModelState.IsValid)
                return View(model);
            
            question.Answers.Add(new Answer
            {
                Content = model.Content,
                IsCorrect = model.IsCorrect
            });

            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", "Questions", new { id });
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult DeleteAnswer(int id, int questionId)
        {
            var answer = _educationPortalDbContext.Answers.FirstOrDefault(x => x.Id == id);

            if (answer == null)
            {
                return RedirectToAction("Details", "Questions", new { id = questionId });
            }

            _educationPortalDbContext.Answers.Remove(answer);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Details", "Questions", new { id = questionId });
        }
    }
}
