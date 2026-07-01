using System.Text.Json;
using Education.Consts;
using Education.DAL;
using Education.DAL.Models;
using Education.Extensions;
using Education.Helpers;
using Education.Models.QuestionAnswers;
using Education.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Education.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = RoleConstants.Teacher)]
public class TeacherController(ApplicationContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserProtocols(long practicalId)
    {
        var grouped = await context.TestResults
            .Include(tr => tr.User)
            .Include(tr => tr.PracticalMaterial)
            .AsNoTracking()
            .Where(tr => tr.PracticalMaterialId == practicalId && tr.IsCompleted)
            .GroupBy(tr => tr.UserId)
            .ToListAsync();


        var result = new List<UserProtocol>();

        foreach (var group in grouped)
        {
            var (grade, graded) = await GetUserGrade(group.Key, practicalId);
            var userProtocol = new UserProtocol
            {
                UserId = group.Key,
                Name = group.First().User.FirstName,
                IsGraded = graded,
                Grade = grade
            };

            foreach (var testResult in group)
            {
                var testProtocol = new TestProtocol
                {
                    Id = testResult.Id,
                    Score = testResult.Score,
                    MaxScore = testResult.MaxScore,
                    TryNumber = testResult.TryNumber,
                    PercentForFive = testResult.PracticalMaterial.PercentForFive,
                    PercentForFour = testResult.PracticalMaterial.PercentForFour,
                    PercentForThree = testResult.PracticalMaterial.PercentForThree,
                    Grade = ScoreHelper.GetGrade(testResult.Score, testResult.MaxScore,
                        testResult.PracticalMaterial.PercentForFive, testResult.PracticalMaterial.PercentForFour,
                        testResult.PracticalMaterial.PercentForThree),
                };

                userProtocol.TestProtocols.Add(testProtocol);
            }

            result.Add(userProtocol);
        }

        return Ok(result);
    }

    class UserProtocol
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public bool IsGraded { get; set; }
        public int Grade { get; set; }
        public List<TestProtocol> TestProtocols { get; set; } = new();
    }

    class TestProtocol
    {
        public long Id { get; set; }
        public double? Score { get; set; }
        public double? MaxScore { get; set; }
        public double TryNumber { get; set; }
        public int Grade { get; set; }
        public double PercentForFive { get; set; }
        public double PercentForFour { get; set; }
        public double PercentForThree { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> GetTestProtocol(long testResultId)
    {
        var testResult = await context.TestResults
            .AsNoTracking()
            .Include(tr => tr.PracticalMaterial)
            .Where(tr => tr.Id == testResultId)
            .Select(tr => new
            {
                tr.Id,
                tr.Score,
                tr.MaxScore,
                Grade = ScoreHelper.GetGrade(tr.Score, tr.MaxScore, tr.PracticalMaterial.PercentForFive, tr.PracticalMaterial.PercentForFour, tr.PracticalMaterial.PercentForThree)
            })
            .FirstOrDefaultAsync();
        if (testResult is null)
        {
            return NotFound();
        }

        JsonSerializerOptions options = new() { AllowOutOfOrderMetadataProperties = true };
        var answersDb = await context.Answers
            .AsNoTracking()
            .Where(a => a.TestResultId == testResult.Id)
            .ToListAsync();

        var answers = answersDb
            .Select(a => JsonSerializer.Deserialize<AnswerBase>(a.Answers, options)).ToList();

        return Ok(new { Answers = answers, testResult.Score, testResult.MaxScore, testResult.Grade });
    }

    [HttpGet]
    public async Task<IActionResult> GetPracticals(long moduleId)
    {
        var result = await context.PracticalMaterials
            .AsNoTracking()
            .Where(m => m.ModuleId == moduleId)
            .Select(m => new { m.Id, m.Name })
            .ToListAsync();
        return Ok(result);
    }


    [HttpGet]
    public async Task<IActionResult> GetTasks(long practicalId)
    {
        var result = await context.Cases
            .AsNoTracking()
            .Where(c => c.PracticalMaterialId == practicalId)
            .Select(c => new { c.Id, c.Name, c.Text })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTheories(long moduleId)
    {
        var result = await context.TheoreticalMaterials
            .AsNoTracking()
            .Where(m => m.ModuleId == moduleId)
            .Select(m => new { m.Id, m.Name })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskFiles(long taskId)
    {
        var result = await context.CaseFiles
            .Include(cf => cf.User)
            .Include(cf => cf.Comments)
            .AsNoTracking()
            .Where(cf => cf.CaseId == taskId)
            .Select(cf => new
            {
                cf.Id,
                cf.Path,
                Name = cf.Path.GetPublicFileName(),
                cf.UserId,
                FullName = cf.User.GetFullName(),
                cf.IsAccepted,
                cf.Grade,
                IsUpdated = cf.Comments.OrderBy(c => c.Id).Last().IsGenerated,
                Comments = cf.Comments.OrderBy(c => c.Id).Select(c => new { c.Id, c.IsGenerated, c.Text, c.Created })
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPracticalTaskFiles(long practicalId)
    {
        var result = await context.CaseFiles
            .Include(cf => cf.User)
            .Include(cf => cf.Comments)
            .Include(cf => cf.Case)
            .AsNoTracking()
            .Where(cf => cf.Case.PracticalMaterialId == practicalId)
            .Select(cf => new
            {
                cf.Id,
                cf.Path,
                Name = cf.Path.GetPublicFileName(),
                cf.UserId,
                FullName = cf.User.GetFullName(),
                cf.IsAccepted,
                cf.Grade,
                IsUpdated = cf.Comments.OrderBy(c => c.Id).Last().IsGenerated,
                Comments = cf.Comments.OrderBy(c => c.Id).Select(c => new { c.Id, c.IsGenerated, c.Text, c.Created })
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents(long courseId)
    {
        var result = await context.Users
            .Include(u => u.Role)
            .Include(u => u.CourseBindUsers)
            .Where(u => u.Role.Name == RoleConstants.Student)
            .AsNoTracking()
            .Select(u => new
            {
                Value = u.Id,
                Label = u.GetFullName(),
                IsEnrolled = u.CourseBindUsers.Any(c => c.CourseId == courseId),
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPracticalStudents(long practicalId)
    {
        var courseId = await context.PracticalMaterials
            .Include(pm => pm.Module)
            .Where(pm => pm.Id == practicalId)
            .Select(pm => pm.Module.CourseId)
            .FirstOrDefaultAsync();

        var result = await context.Users
            .Include(u => u.Role)
            .Include(u => u.PracticalBindUsers)
            .Include(u => u.CourseBindUsers)
            .Where(u => u.Role.Name == RoleConstants.Student && u.CourseBindUsers.Any(c => c.UserId == u.Id && c.CourseId == courseId))
            .AsNoTracking()
            .Select(u => new
            {
                Value = u.Id,
                Label = u.GetFullName(),
                IsEnrolled = u.PracticalBindUsers.Any(c => c.PracticalMaterialId == practicalId),
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions(long moduleId)
    {
        var result = await context.Questions
            .Where(q => q.ModuleId == moduleId)
            .AsNoTracking()
            .Select(q => new
            {
                q.Id,
                q.Text,
                Type = q.QuestionTypeId,
                q.Weight,
                q.Answer
            })
            .OrderByDescending(q => q.Id)
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMakePracticalQuestions(long moduleId, long practId)
    {
        var result = await context.Questions
            .Include(q => q.PracticalMaterialBindQuestions)
            .Where(q => q.ModuleId == moduleId)
            .AsNoTracking()
            .Select(q => new
            {
                q.Id,
                q.Text,
                Type = q.QuestionTypeId,
                Body = q.Options,
                IsSelected = q.PracticalMaterialBindQuestions.Any(q => q.PracticalMaterialId == practId)
            })
            .ToListAsync();

        var practical = await context.PracticalMaterials.FirstOrDefaultAsync(pm => pm.Id == practId);
        if (practical is null)
        {
            return NotFound();
        }

        return Ok(new { Questions = result, practical.IsPublic, practical.TriesCount, practical.PercentForFive, practical.PercentForFour, practical.PercentForThree });
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var result = await context.Courses
            .Include(c => c.User)
            .Where(c => c.User.Login == login)
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                c.Date,
                c.Description,
                c.Name,
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] AddCourseRequest request)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user is null)
        {
            return BadRequest();
        }

        var course = new Course()
        {
            Name = request.Name,
            Description = request.Description,
            Date = request.Date.UtcDateTime,
            UserId = user.Id
        };
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();
        return Ok(new
        {
            course.Id,
            course.Date,
            course.Description,
            course.Name,
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCourseStudents([FromBody] UpdateCourseStudentsRequest request)
    {
        var curStudentsOnCourse = await context.Users
            .Include(u => u.Role)
            .Include(u => u.CourseBindUsers)
            .Where(u => u.Role.Name == RoleConstants.Student && u.CourseBindUsers.Any(c => c.CourseId == request.CourseId))
            .ToListAsync();

        foreach (var student in curStudentsOnCourse)
        {
            if (!request.UserIds.Contains(student.Id))
            {
                var courseBind = await context.CourseBindUsers.FirstOrDefaultAsync(c => c.CourseId == request.CourseId && c.UserId == student.Id);
                if (courseBind is not null)
                {
                    context.CourseBindUsers.Remove(courseBind);
                }
            }
            else
            {
                request.UserIds.Remove(student.Id);
            }
        }

        await context.SaveChangesAsync();

        var courseBinds = request.UserIds.Select(studId => new CourseBindUser()
        {
            CourseId = request.CourseId,
            UserId = studId
        }).ToList();

        await context.CourseBindUsers.AddRangeAsync(courseBinds);

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePracticalStudents([FromBody] UpdatePracticalStudentsRequest request)
    {
        var curStudentOnPractical = await context.Users
            .Include(u => u.Role)
            .Include(u => u.PracticalBindUsers)
            .Where(u => u.Role.Name == RoleConstants.Student && u.PracticalBindUsers.Any(c => c.PracticalMaterialId == request.PracticalId))
            .ToListAsync();

        foreach (var student in curStudentOnPractical)
        {
            if (!request.UserIds.Contains(student.Id))
            {
                var practicalBind = await context.PracticalBindUsers.FirstOrDefaultAsync(c => c.PracticalMaterialId == request.PracticalId && c.UserId == student.Id);
                if (practicalBind is not null)
                {
                    context.PracticalBindUsers.Remove(practicalBind);
                }
            }
            else
            {
                request.UserIds.Remove(student.Id);
            }
        }

        await context.SaveChangesAsync();

        var practicalBinds = request.UserIds.Select(studId => new PracticalBindUser()
        {
            PracticalMaterialId = request.PracticalId,
            UserId = studId
        }).ToList();

        await context.PracticalBindUsers.AddRangeAsync(practicalBinds);

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTheoryTitle([FromBody] UpdateTheoryTitleRequest request)
    {
        var theory = await context.TheoreticalMaterials.FirstOrDefaultAsync(m => m.Id == request.TheoryId);
        if (theory is null)
        {
            return BadRequest();
        }

        theory.Name = request.Title;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePracticalMaterialQuestions(
        [FromBody] UpdatePracticalQuestionsRequest request)
    {
        var practical = await context.PracticalMaterials.FirstOrDefaultAsync(pm => pm.Id == request.PracticalId);
        if (practical is null)
        {
            return BadRequest();
        }

        practical.TriesCount = request.TriesCount;
        practical.PercentForFive = request.PercentForFive;
        practical.PercentForFour = request.PercentForFour;
        practical.PercentForThree = request.PercentForThree;

        var curTestQuestions = await context.PracticalMaterialBindQuestions
            .Where(u => u.PracticalMaterialId == request.PracticalId)
            .ToListAsync();

        foreach (var bind in curTestQuestions)
        {
            if (!request.QuestionIds.Contains(bind.QuestionId))
            {
                context.PracticalMaterialBindQuestions.Remove(bind);
            }
            else
            {
                request.QuestionIds.Remove(bind.QuestionId);
            }
        }

        await context.SaveChangesAsync();


        var questBinds = request.QuestionIds.Select(questId => new PracticalMaterialBindQuestion()
        {
            PracticalMaterialId = request.PracticalId,
            QuestionId = questId
        }).ToList();

        await context.PracticalMaterialBindQuestions.AddRangeAsync(questBinds);

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionRequest request)
    {
        var question = await context.Questions
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId);

        if (question is null)
        {
            return NotFound();
        }

        question.Answer = request.Answer;
        question.Options = request.Body;
        question.Weight = request.Weight;
        question.Text = request.Text;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTheoryText([FromBody] UpdateTheoryTextRequest request)
    {
        var theory = await context.TheoreticalMaterials.FirstOrDefaultAsync(m => m.Id == request.TheoryId);
        if (theory is null)
        {
            return BadRequest();
        }

        theory.Text = request.Text;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTaskText([FromBody] UpdateTaskTextRequest request)
    {
        var task = await context.Cases.FirstOrDefaultAsync(c => c.Id == request.TaskId);
        if (task is null)
        {
            return BadRequest();
        }

        task.Text = request.Text;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> MakePracticalPublic(long practicalId)
    {
        var practical = await context.PracticalMaterials.FirstOrDefaultAsync(p => p.Id == practicalId);
        if (practical is null)
        {
            return BadRequest();
        }

        practical.IsPublic = true;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> AcceptTaskFile(long taskFileId, int grade)
    {
        var taskFile = await context.CaseFiles.FirstOrDefaultAsync(f => f.Id == taskFileId);
        if (taskFile is null)
        {
            return BadRequest();
        }

        taskFile.IsAccepted = true;
        taskFile.Grade = grade;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateModule([FromBody] AddModuleRequest request)
    {
        var module = new Module()
        {
            CourseId = request.CourseId,
            Name = request.Name
        };
        await context.Modules.AddAsync(module);
        await context.SaveChangesAsync();
        return Ok(new { module.Id, module.Name });
    }

    [HttpPost]
    public async Task<IActionResult> CreatePractical([FromBody] AddPracticalRequest request)
    {
        var practical = new PracticalMaterial()
        {
            ModuleId = request.ModuleId,
            Name = request.Name,
            TriesCount = 1,
            PercentForFive = 90,
            PercentForFour = 75,
            PercentForThree = 60
        };
        await context.PracticalMaterials.AddAsync(practical);
        await context.SaveChangesAsync();
        return Ok(new { practical.Id, practical.Name });
    }

    [HttpPost]
    public async Task<IActionResult> AddTaskFileComment([FromBody] AddTaskFileCommentRequest request)
    {
        var comment = new CaseFileComment()
        {
            CaseFileId = request.TaskFileId,
            IsGenerated = false,
            Text = request.Comment,
        };
        await context.CaseFileComments.AddAsync(comment);
        await context.SaveChangesAsync();
        return Ok(new { comment.Id, comment.Text, comment.Created, comment.IsGenerated });
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion([FromBody] AddQuestionRequest request)
    {
        var question = new Question()
        {
            ModuleId = request.ModuleId,
            Answer = request.Answer,
            Options = request.Body,
            Weight = request.Weight,
            Text = request.Text,
            QuestionTypeId = (long)request.Type
        };
        await context.Questions.AddAsync(question);
        await context.SaveChangesAsync();
        return Ok(new
        {
            question.Id,
            question.Text,
            Type = question.QuestionTypeId,
            question.Weight,
            question.Answer
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTest([FromBody] AddTestRequest request)
    {
        var practicalMaterial = await context.PracticalMaterials.FirstOrDefaultAsync(pm => pm.Id == request.PracticalId);
        if (practicalMaterial is null)
        {
            return BadRequest();
        }

        var binds = request.QuestionIds.Select(id => new PracticalMaterialBindQuestion()
        {
            PracticalMaterialId = practicalMaterial.Id,
            QuestionId = id
        });
        await context.PracticalMaterialBindQuestions.AddRangeAsync(binds);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTheory([FromBody] AddTheoryRequest request)
    {
        TheoreticalMaterial theory = new() { ModuleId = request.ModuleId, Name = request.Name, Text = "Текст лекции" };
        await context.TheoreticalMaterials.AddAsync(theory);
        await context.SaveChangesAsync();
        return Ok(new { theory.Id, theory.Name });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] AddTaskRequest request)
    {
        Case task = new() { PracticalMaterialId = request.PracticalId, Name = request.Name, Text = "Текст задания" };
        await context.Cases.AddAsync(task);
        await context.SaveChangesAsync();
        return Ok(new { task.Id, task.Name });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTheoryDocument([FromForm] AddDocRequest request)
    {
        var fileName = await FileHelper.SaveFileToPublic(request.File);
        var file = new TheoreticalMaterialFile()
        {
            Path = fileName,
            Description = request.Descritpion,
            TheoreticalMaterialId = request.TheoryMaterialId
        };
        await context.TheoreticalMaterialFiles.AddAsync(file);
        await context.SaveChangesAsync();
        return Ok(new { file.Id, file.Description, file.Path, Name = file.Path.GetPublicFileName() });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTheoryLink([FromBody] AddLinkRequest request)
    {
        var link = new TheoreticalMaterialLink()
        {
            Link = request.Link,
            Description = request.Description,
            TheoreticalMaterialId = request.TheoryMaterialId
        };
        await context.TheoreticalMaterialLinks.AddAsync(link);
        await context.SaveChangesAsync();
        return Ok(new { link.Id, link.Description, link.Link });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCourse(long courseId)
    {
        await context.Courses.Where(c => c.Id == courseId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteModule(long moduleId)
    {
        await context.Modules.Where(c => c.Id == moduleId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeletePractical(long practicalId)
    {
        await context.PracticalMaterials.Where(c => c.Id == practicalId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteQuestion(long questionId)
    {
        await context.Questions.Where(q => q.Id == questionId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTheoryMaterial(long theoryId)
    {
        await context.TheoreticalMaterials.Where(q => q.Id == theoryId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTask(long taskId)
    {
        await context.Cases.Where(q => q.Id == taskId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTheoryLink(long linkId)
    {
        await context.TheoreticalMaterialLinks.Where(q => q.Id == linkId).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTheoryDoc(long docId)
    {
        var theorFile = await context.TheoreticalMaterialFiles.FirstOrDefaultAsync(f => f.Id == docId);
        if (theorFile is null)
        {
            return BadRequest();
        }

        FileHelper.DeleteFileFromPublic(theorFile.Path);
        context.TheoreticalMaterialFiles.Remove(theorFile);
        await context.SaveChangesAsync();
        return Ok();
    }

    private async Task<(int, bool)> GetUserGrade(long userId, long practicalId)
    {
        var casesPractical = await context.Cases.CountAsync(c => c.PracticalMaterialId == practicalId);
        var casesPracticalUser = await context.CaseFiles.Include(cf => cf.Case).CountAsync(cf => cf.Case.PracticalMaterialId == practicalId && cf.UserId == userId && cf.IsAccepted);

        List<string> messages = new();
        if (casesPractical != casesPracticalUser)
        {
            messages.Add("Выполните все задания");
        }

        var isTestResult = await context.TestResults.AnyAsync(t => t.PracticalMaterialId == practicalId && t.UserId == userId);
        if (!isTestResult)
        {
            messages.Add("Пройдите тестирование");
        }

        if (messages.Count != 0)
        {
            return (0, false);
        }

        var testResults = await context.TestResults.Include(tr => tr.PracticalMaterial).Where(t => t.PracticalMaterialId == practicalId && t.UserId == userId).ToListAsync();
        var bestTestScore = testResults.Select(tr => ScoreHelper.GetGrade(tr.Score, tr.MaxScore,
            tr.PracticalMaterial.PercentForFive, tr.PracticalMaterial.PercentForFour,
            tr.PracticalMaterial.PercentForThree)).Max();

        var meanTaskScore = await context.CaseFiles.Include(cf => cf.Case)
            .Where(cf => cf.Case.PracticalMaterialId == practicalId && cf.UserId == userId).Select(cf => cf.Grade)
            .AverageAsync();

        var grade = Math.Ceiling((bestTestScore + meanTaskScore) / 2);
        return ((int)grade, true);
    }
}
