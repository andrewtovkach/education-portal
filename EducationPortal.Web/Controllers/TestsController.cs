using System;
using System.Collections.Generic;
using System.Linq;
using EducationPortal.Web.Data;
using EducationPortal.Web.Data.Entities;
using EducationPortal.Web.Data.Enums;
using EducationPortal.Web.Extensions;
using EducationPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationPortal.Web.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EducationPortalDbContext _educationPortalDbContext;

        public TestsController(EducationPortalDbContext educationPortalDbContext,
            UserManager<IdentityUser> userManager)
        {
            _educationPortalDbContext = educationPortalDbContext;
            _userManager = userManager;
        }

        public IActionResult Details(int id)
        {
            var test = _educationPortalDbContext.Tests.Where(x => x.Id == id)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .Include(x => x.Module)
                .ThenInclude(x => x.Course)
                .FirstOrDefault();

            if (test == null || test.Questions.Count == 0)
            {
                return NotFound();
            }

            var questionViewModels = GetQuestionViewModels(test);

            var testDetailsViewModel = new TestDetailsViewModel
            {
                Questions = questionViewModels,
                TestName = test.Name,
                TestId = test.Id,
                TimeLimit = test.TimeLimit,
                CourseId = test.Module.CourseId,
                CourseName = test.Module.Course.Name,
                ModuleId = test.ModuleId,
                ModuleName = test.Module.Name,
                Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == test.ModuleId),
                EducationMaterials = _educationPortalDbContext.EducationMaterials.Where(x => x.ModuleId == test.ModuleId)
            };

            return View(testDetailsViewModel);
        }

        [HttpPost]
        public IActionResult FinishTest(int id)
        {
            var test = _educationPortalDbContext.Tests.Where(x => x.Id == id)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .FirstOrDefault();

            var form = HttpContext.Request.Form;

            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;
            var attemptId = AddTestAttempt(id, userId);

            AddAnswerHistoryData(test, form, attemptId);
            UpdateTotalScore(test, attemptId);

            return RedirectToAction("TestAttempt", new { id = attemptId, hasNotification = true });
        }

        public IActionResult FinishedTest(int id)
        {
            var test = _educationPortalDbContext.Tests.FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;

            var testCompletion = _educationPortalDbContext.TestCompletions.Where(x => x.TestId == id && x.UserId == Guid.Parse(userId))
                .Include(x => x.Attempts)
                .Include(x => x.Test)
                .ThenInclude(x => x.Module)
                .ThenInclude(x => x.Course)
                .FirstOrDefault();

            if (testCompletion == null)
            {
                return NotFound();
            }

            var testCompletions = _educationPortalDbContext.TestCompletions
                .Include(x => x.Test)
                .Include(x => x.Attempts)
                .Where(x => x.UserId == Guid.Parse(userId) && x.Test.ModuleId == test.ModuleId);

            var finishedTestViewModel = new FinishedTestViewModel
            {
                TestId = test.Id,
                TestName = test.Name,
                Attempts = testCompletion.Attempts.OrderByDescending(x => x.Date),
                CourseName = test.Module.Course.Name,
                CourseId = test.Module.Course.Id,
                ModuleId = test.ModuleId,
                ModuleName = test.Module.Name,
                TestCompletions = testCompletions,
                EducationMaterials = _educationPortalDbContext.EducationMaterials.Where(x => x.ModuleId == test.ModuleId)
            };

            return View(finishedTestViewModel);
        }

        public IActionResult TestAttempt(int id, bool hasNotification = false)
        {
            var attempt = _educationPortalDbContext.Attempts
                .Where(x => x.Id == id)
                .Include(x => x.AnswerHistoryData)
                .ThenInclude(x => x.AnswerHistories)
                .ThenInclude(x => x.Answer)
                .Include(x => x.AnswerHistoryData)
                .ThenInclude(x => x.Question)
                .ThenInclude(x => x.Answers)
                .Include(x => x.TestCompletion)
                .FirstOrDefault();

            if (attempt == null)
            {
                return NotFound();
            }

            var test = _educationPortalDbContext.Tests
                .Include(x => x.Module)
                .ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.Id == attempt.TestCompletion.TestId);

            if (test == null)
            {
                return NotFound();
            }

            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;
            var testCompletion = _educationPortalDbContext.TestCompletions
                .Where(x => x.TestId == test.Id && x.UserId == Guid.Parse(userId))
                .Include(x => x.Attempts)
                .FirstOrDefault();

            if (testCompletion == null)
            {
                return NotFound();
            }

            var attemptViewModel = new AttemptViewModel
            {
                TestName = test.Name,
                TestId = test.Id,
                Attempt = attempt,
                CourseName = test.Module.Course.Name,
                CourseId = test.Module.CourseId,
                ModuleId = test.ModuleId,
                ModuleName = test.Module.Name,
                Attempts = testCompletion.Attempts.OrderByDescending(x => x.Date),
                MaxNumberOfAttempts = test.MaxNumberOfAttempts
            };

            return hasNotification ? View(attemptViewModel).WithSuccess("", "Тест был завершен!") : View(attemptViewModel);
        }

        #region Private Methods
        private static IEnumerable<QuestionViewModel> GetQuestionViewModels(Test test)
        {
            var questionViewModels = new List<QuestionViewModel>();
            var questionNumber = 1;
            foreach (var question in test.Questions)
            {
                questionViewModels.Add(new QuestionViewModel
                {
                    Question = question,
                    QuestionNumber = questionNumber
                });

                questionNumber++;
            }

            return questionViewModels;
        }

        private void UpdateTotalScore(Test test, int attemptId)
        {
            var score = GetScore(test);
            var totalScore = GetTotalScore(test);

            var currentAttempt = _educationPortalDbContext.Attempts.FirstOrDefault(x => x.Id == attemptId);
            if (currentAttempt == null)
            {
                return;
            }

            currentAttempt.Score = (int)((double)score / totalScore * 100.0);

            _educationPortalDbContext.SaveChanges();
        }

        private int AddTestAttempt(int testId, string userId)
        {
            if (!_educationPortalDbContext.TestCompletions.Any(x => x.TestId == testId))
            {
                var newAttempt = new Attempt
                {
                    Date = DateTime.Now,
                    Name = "Результат 1"
                };

                _educationPortalDbContext.TestCompletions.Add(new TestCompletion
                {
                    UserId = Guid.Parse(userId),
                    TestId = testId,
                    Attempts = new List<Attempt>
                    {
                        newAttempt
                    }
                });

                _educationPortalDbContext.SaveChanges();

                return newAttempt.Id;
            }
            else
            {
                var testCompletion = _educationPortalDbContext.TestCompletions.Where(x => x.TestId == testId)
                    .Include(x => x.Attempts)
                    .FirstOrDefault();

                if (testCompletion == null)
                {
                    return -1;
                }

                var newAttempt = new Attempt
                {
                    Date = DateTime.Now,
                    Name = $"Результат {testCompletion.Attempts.Count() + 1}"
                };

                testCompletion.Attempts.Add(newAttempt);
                _educationPortalDbContext.SaveChanges();

                return newAttempt.Id;
            }
        }

        private void AddAnswerHistoryData(Test test, IFormCollection form, int attemptId)
        {
            foreach (var question in test.Questions)
            {
                if (question.QuestionType == QuestionType.OneAnswer ||
                    question.QuestionType == QuestionType.MultipleAnswers)
                {
                    var answerHistories = form[question.Id.ToString()].Select(x =>
                    {
                        var answer = question.Answers.FirstOrDefault(a => a.Id == Convert.ToInt32(x));
                        if (answer == null)
                        {
                            return null;
                        }

                        return new AnswerHistory
                        {
                            AnswerId = Convert.ToInt32(x),
                            IsCorrect = answer.IsCorrect
                        };
                    });


                     _educationPortalDbContext.AnswerHistoryData.Add(new AnswerHistoryData
                     {
                         AttemptId = attemptId,
                         Date = DateTime.Now,
                         QuestionId = question.Id,
                         AnswerHistories = answerHistories.ToList()
                     });
                }
                else
                {
                    var answer = question.Answers.FirstOrDefault();

                    if (answer == null)
                    {
                        return;
                    }

                    var textInput = form[question.Id.ToString()];
                    var answerHistories = new List<AnswerHistory>
                    {
                        new AnswerHistory
                        {
                            AnswerId = answer.Id,
                            IsCorrect = !string.IsNullOrEmpty(textInput) && textInput == answer.Content
                        }
                    };

                    if (!string.IsNullOrEmpty(textInput))
                    {
                        answerHistories.First().TextInput = textInput;
                    }

                    _educationPortalDbContext.AnswerHistoryData.Add(new AnswerHistoryData
                    {
                        AttemptId = attemptId,
                        Date = DateTime.Now,
                        QuestionId = question.Id,
                        AnswerHistories = answerHistories
                    });
                }
            }

            _educationPortalDbContext.SaveChanges();
        }

        private int GetScore(Test test)
        {
            return test.Questions.Select(question => _educationPortalDbContext.AnswerHistoryData.Where(x => x.QuestionId == question.Id)
                    .OrderByDescending(x => x.Date)
                    .Include(x => x.AnswerHistories)
                    .FirstOrDefault())
                .Where(answerHistoryData => answerHistoryData != null)
                .Sum(answerHistoryData => answerHistoryData.AnswerHistories.Count(x => x.IsCorrect));
        }

        private static int GetTotalScore(Test test)
        {
            return test.Questions.Sum(question => question.Answers.Count(x => x.IsCorrect));
        }
        #endregion
    }
}
