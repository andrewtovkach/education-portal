using System;
using System.Collections.Generic;
using System.Linq;
using EducationPortal.Web.Data;
using EducationPortal.Web.Data.Entities;
using EducationPortal.Web.Data.Enums;
using EducationPortal.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationPortal.Web.Controllers
{
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

        public IActionResult Details(int? id)
        {
            var test = _educationPortalDbContext.Tests.Where(x => x.Id == id)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .FirstOrDefault();

            var testDetailsViewModel = new TestDetailsViewModel
            {
                Questions = test?.Questions,
                TestName = test?.Name,
                TestId = test.Id
            };

            return View(testDetailsViewModel);
        }

        [HttpPost]
        public IActionResult FinishTest(int? id)
        {
            var test = _educationPortalDbContext.Tests.Where(x => x.Id == id)
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .FirstOrDefault();

            var form = HttpContext.Request.Form;

            var userId = _userManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name)?.Id;
            var attemptId = AddTestAttempt(id);

            AddAnswerHistoryData(test, form, userId, attemptId);

            CalculateTotalScore(test, attemptId);

            return RedirectToAction("FinishedTest", new {id = attemptId });
        }

        private void CalculateTotalScore(Test test, int attemptId)
        {
            var totalScore = GetTotalScore(test);

            var currentAttempt = _educationPortalDbContext.Attempts.FirstOrDefault(x => x.Id == attemptId);
            currentAttempt.Score = totalScore;

            _educationPortalDbContext.SaveChanges();
        }

        public IActionResult FinishedTest(int? id)
        {
            var attempt = _educationPortalDbContext.Attempts.Where(x => x.Id == id)
                .Include(x => x.AnswerHistoryData)
                .ThenInclude(x => x.Question)
                .Include(x => x.AnswerHistoryData)
                .ThenInclude(x => x.AnswerHistories)
                .ThenInclude(x => x.Answer)
                .FirstOrDefault();

            return View(attempt.AnswerHistoryData);
        }

        private int AddTestAttempt(int? testId)
        {
            if (!_educationPortalDbContext.TestCompletions.Any(x => x.TestId == testId))
            {
                var newAttempt = new Attempt
                {
                    Date = DateTime.Now,
                    Name = "Attempt #1"
                };

                _educationPortalDbContext.TestCompletions.Add(new TestCompletion
                {
                    TestId = testId.Value,
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

                var newAttempt = new Attempt
                {
                    Date = DateTime.Now,
                    Name = $"Attempt #{testCompletion.Attempts.Count() + 1}"
                };

                testCompletion.Attempts.Add(newAttempt);
                _educationPortalDbContext.SaveChanges();

                return newAttempt.Id;
            }
        }

        private void AddAnswerHistoryData(Test test, IFormCollection form, string userId, int attemptId)
        {
            foreach (var question in test.Questions)
            {
                if (question.QuestionType == QuestionType.OneAnswer ||
                    question.QuestionType == QuestionType.MultipleAnswers)
                {
                    var answerHistories = form[question.Id.ToString()].Select(x => new AnswerHistory
                    {
                        AnswerId = Convert.ToInt32(x),
                        IsCorrect = question.Answers.FirstOrDefault(a => a.Id == Convert.ToInt32(x)).IsCorrect,
                        NumberOfPoints = question.Answers.FirstOrDefault(a => a.Id == Convert.ToInt32(x)).IsCorrect
                            ? question.Answers.FirstOrDefault(a => a.Id == Convert.ToInt32(x)).NumberOfPoints
                            : 0,
                    });

                    _educationPortalDbContext.AnswerHistoryData.Add(new AnswerHistoryData
                    {
                        AttemptId = attemptId,
                        Date = DateTime.Now,
                        QuestionId = question.Id,
                        UserId = Guid.Parse(userId),
                        AnswerHistories = answerHistories.ToList()
                    });
                }
                else
                {
                    var answer = question.Answers.FirstOrDefault();

                    _educationPortalDbContext.AnswerHistoryData.Add(new AnswerHistoryData
                    {
                        AttemptId = attemptId,
                        Date = DateTime.Now,
                        QuestionId = question.Id,
                        UserId = Guid.Parse(userId),
                        AnswerHistories = new List<AnswerHistory>
                        {
                            new AnswerHistory
                            {
                                AnswerId = answer.Id,
                                TextInput = form[question.Id.ToString()],
                                IsCorrect = form[question.Id.ToString()] == answer.Content,
                                NumberOfPoints = form[question.Id.ToString()] == answer.Content ? answer.NumberOfPoints : 0
                            }
                        }
                    });
                }
            }

            _educationPortalDbContext.SaveChanges();
        }

        private int GetTotalScore(Test test)
        {
            var totalScore = 0;
            foreach (var question in test.Questions)
            {
                var answerHistoryData = _educationPortalDbContext.AnswerHistoryData
                    .Where(x => x.QuestionId == question.Id)
                    .OrderByDescending(x => x.Date)
                    .Include(x => x.AnswerHistories)
                    .FirstOrDefault();

                var questionScore = answerHistoryData.AnswerHistories.Sum(x => x.NumberOfPoints);
                totalScore += questionScore;
            }

            return totalScore;
        }
    }
}
