using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly ApplicationDbContext _educationPortalDbContext;

        public TestsController(ApplicationDbContext educationPortalDbContext,
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
                Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == test.ModuleId)
            };

            return View(testDetailsViewModel);
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult Info(int id)
        {
            var test = _educationPortalDbContext.Tests
                .Include(x => x.Module)
                .ThenInclude(x => x.Course)
                .Include(x => x.Questions)
                .FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = test.Module.CourseId;
            ViewBag.CourseName = test.Module.Course.Name;
            ViewBag.ModuleId = test.ModuleId;
            ViewBag.ModuleName = test.Module.Name;
            ViewBag.Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == test.ModuleId);

            var createTestViewModel = new CreateTestDetailsViewModel
            {
                Id = id,
                Name = test.Name,
                Questions = test.Questions
            };

            return View(createTestViewModel);
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddQuestion(int id)
        {
            var test = _educationPortalDbContext.Tests
                .Include(x => x.Module)
                .ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = test.Module.CourseId;
            ViewBag.CourseName = test.Module.Course.Name;
            ViewBag.ModuleId = test.ModuleId;
            ViewBag.ModuleName = test.Module.Name;
            ViewBag.TestId = test.Id;
            ViewBag.TestName = test.Name;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, tutor")]
        public IActionResult AddQuestion(CreateQuestionViewModel model, int id)
        {
            var test = _educationPortalDbContext.Tests
                .Include(x => x.Module)
                .ThenInclude(x => x.Course)
                .FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = test.Module.CourseId;
            ViewBag.CourseName = test.Module.Course.Name;
            ViewBag.ModuleId = test.ModuleId;
            ViewBag.ModuleName = test.Module.Name;
            ViewBag.TestId = test.Id;
            ViewBag.TestName = test.Name;

            if (!ModelState.IsValid)
                return View(model);

            if (model.File != null)
            {
                if (!model.File.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("File", "Пожалуйста выберите картинку!");
                    return View(model);
                }
            }

            Question question = null;

            if (model.File != null)
            {
                question = UploadImageToDb(model, test);
            }
            else
            {
                question = new Question
                {
                    Content = model.Content,
                    QuestionType = model.QuestionType.Value
                };

                test.Questions.Add(question);

                _educationPortalDbContext.SaveChanges();
            }

            return RedirectToAction("Details", "Questions", new { id = question.Id });
        }

        [Authorize(Roles = "admin, tutor")]
        public IActionResult DeleteQuestion(int id, int testId)
        {
            var question = _educationPortalDbContext.Questions.FirstOrDefault(x => x.Id == id);

            if (question == null)
            {
                return RedirectToAction("Info", "Tests", new { id = testId });
            }

            _educationPortalDbContext.Questions.Remove(question);
            _educationPortalDbContext.SaveChanges();

            return RedirectToAction("Info", "Tests", new { id = testId });
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

        public IActionResult AllResults(int id)
        {
            var test = _educationPortalDbContext.Tests
                .Include(x => x.Module)
                .ThenInclude(x => x.Course)
                .Include(x => x.Questions)
                .FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = test.Module.CourseId;
            ViewBag.CourseName = test.Module.Course.Name;
            ViewBag.ModuleId = test.ModuleId;
            ViewBag.ModuleName = test.Module.Name;
            ViewBag.Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == test.ModuleId);

            var testCompeletions = _educationPortalDbContext.TestCompletions.Where(x => x.TestId == id)
                .Include(x => x.Attempts).ToList();

            var testResults = testCompeletions.Select(testCompletion => new TestCompeletionViewModel
            {
                UserName = _userManager.FindByIdAsync(testCompletion.UserId.ToString()).GetAwaiter().GetResult().UserName,
                AverageResult = testCompletion.Attempts.Average(x => x.Score),
                LastAttempt = testCompletion.Attempts.Last().Date
            });

            var model = new TestCompletionNavViewModel()
            {
                TestCompeletions = testResults,
                TestId = id,
                TestName = _educationPortalDbContext.Tests.FirstOrDefault(x => x.Id == id).Name
            };

            return View(model);
        }

        public IActionResult Statistics(int id)
        {
            var test = _educationPortalDbContext.Tests
                            .Include(x => x.Module)
                            .ThenInclude(x => x.Course)
                            .Include(x => x.Questions)
                            .FirstOrDefault(x => x.Id == id);

            if (test == null)
            {
                return NotFound();
            }

            ViewBag.CourseId = test.Module.CourseId;
            ViewBag.CourseName = test.Module.Course.Name;
            ViewBag.ModuleId = test.ModuleId;
            ViewBag.ModuleName = test.Module.Name;
            ViewBag.Tests = _educationPortalDbContext.Tests.Where(x => x.ModuleId == test.ModuleId);

            var testCompeletions = _educationPortalDbContext.TestCompletions.Where(x => x.TestId == id)
                .Include(x => x.Attempts).ToList();

            var chartValues = testCompeletions.Select(testCompletion => new TestCompeletionViewModel
            {
                AverageResult = testCompletion.Attempts.Average(x => x.Score)
            }).GroupBy(x => (int)(x.AverageResult / 10))
            .Select(group => new ChartValueViewModel
            {
                Name = $"{group.Key * 10} - {group.Key * 10 + 10}%",
                Y = group.Count()
            });

            return View(new StatisticsViewModel
            {
                TestId = test.Id,
                TestName = test.Name,
                Names = chartValues.Select(x => x.Name),
                ChartValues = chartValues
            });
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
            return _educationPortalDbContext.TestCompletions.Any(x => x.TestId == testId && x.UserId == Guid.Parse(userId)) ? 
                AddAdditionalAttempt(testId, userId) : AddNewTestAttempt(testId, userId);
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

        private int AddAdditionalAttempt(int testId, string userId)
        {
            var testCompletion = _educationPortalDbContext.TestCompletions
                .Where(x => x.TestId == testId && x.UserId == Guid.Parse(userId))
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

        private int AddNewTestAttempt(int testId, string userId)
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

        private Question UploadImageToDb(CreateQuestionViewModel model, Test test)
        {
            using (var ms = new MemoryStream())
            {
                model.File.CopyTo(ms);
                var fileBytes = ms.ToArray();

                var question = new Question
                {
                    Image = fileBytes,
                    ImageContentType = model.File.ContentType,
                    QuestionType = model.QuestionType.Value,
                    Content = model.Content
                };

                test.Questions.Add(question);
                _educationPortalDbContext.SaveChanges();

                return question;
            }
        }
        #endregion
    }
}
