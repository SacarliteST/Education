using System.Diagnostics;
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
[Authorize(Roles = RoleConstants.Student)]
[Route("api/[controller]/[action]")]
public class StudentController(ApplicationContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();
        var result = await context.Courses
            .Include(c => c.CourseBindUsers)
            .Where(c => c.CourseBindUsers.Any(uc => uc.UserId == userId))
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

    [HttpGet]
    public async Task<IActionResult> GetPracticalGrade(long practicalId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();
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
            return Ok(new { Messages = messages });
        }

        var testResults = await context.TestResults.Include(tr => tr.PracticalMaterial).Where(t => t.PracticalMaterialId == practicalId && t.UserId == userId).ToListAsync();
        var bestTestScore = testResults.Select(tr => ScoreHelper.GetGrade(tr.Score, tr.MaxScore,
            tr.PracticalMaterial.PercentForFive, tr.PracticalMaterial.PercentForFour,
            tr.PracticalMaterial.PercentForThree)).Max();

        try
        {
            var meanTaskScore = await context.CaseFiles.Include(cf => cf.Case)
                .Where(cf => cf.Case.PracticalMaterialId == practicalId && cf.UserId == userId).Select(cf => cf.Grade)
                .AverageAsync();

            var grade = Math.Ceiling((bestTestScore + meanTaskScore) / 2);
            return Ok(new { Grade = grade });
        }
        catch
        {
            return Ok(new { Grade = Math.Ceiling((double)bestTestScore) });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProtocols(long practicalId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();

        var result = await context.TestResults
            .Include(tr => tr.User)
            .Include(tr => tr.PracticalMaterial)
            .AsNoTracking()
            .Where(tr => tr.PracticalMaterialId == practicalId && tr.IsCompleted && tr.UserId == userId)
            .Select(tr => new
            {
                tr.Id,
                tr.UserId,
                tr.Score,
                tr.MaxScore,
                tr.TryNumber,
                Grade = ScoreHelper.GetGrade(tr.Score, tr.MaxScore, tr.PracticalMaterial.PercentForFive, tr.PracticalMaterial.PercentForFour, tr.PracticalMaterial.PercentForThree)
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProtocol(long testResultId)
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
                tr.IsCompleted,
                tr.TryNumber,
                Grade = ScoreHelper.GetGrade(tr.Score, tr.MaxScore, tr.PracticalMaterial.PercentForFive, tr.PracticalMaterial.PercentForFour, tr.PracticalMaterial.PercentForThree)
            })
            .FirstOrDefaultAsync();
        if (testResult is null or { IsCompleted: false })
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

        return Ok(new
        {
            Answers = answers,
            testResult.TryNumber,
            testResult.Score,
            testResult.MaxScore,
            testResult.Grade
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetPracticalQuestions(long practId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();

        var testResult = await context.TestResults.Where(tr => tr.UserId == userId && tr.PracticalMaterialId == practId && !tr.IsCompleted).FirstOrDefaultAsync();
        if (testResult is null or { IsCompleted: true })
        {
            return NotFound();
        }

        var isCompleted = testResult.IsCompleted;
        var result = await context.Questions
            .Include(q => q.PracticalMaterialBindQuestions)
            .Where(q => q.PracticalMaterialBindQuestions.Any(pq => pq.PracticalMaterialId == practId))
            .AsNoTracking()
            .Select(q => new
            {
                q.Id,
                q.Text,
                Type = q.QuestionTypeId,
                Body = q.Options
            })
            .OrderByDescending(q => q.Id)
            .ToListAsync();

        return Ok(new { Questions = result, IsCompleted = isCompleted });
    }

    [HttpGet]
    public async Task<IActionResult> GetTestStatus(long practicalId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();

        var testRes = await context.TestResults.AsNoTracking().FirstOrDefaultAsync(tr => tr.PracticalMaterialId == practicalId && tr.UserId == userId && !tr.IsCompleted);
        if (testRes is not null)
        {
            return Ok(new { IsStarted = true, testRes.TryNumber });
        }

        return Ok(new { IsStarted = false });
    }

    [HttpPut]
    public async Task<IActionResult> StartTest(long practicalId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();

        var testRes = await context.TestResults.FirstOrDefaultAsync(tr => tr.PracticalMaterialId == practicalId && tr.UserId == userId && !tr.IsCompleted);
        if (testRes is null)
        {
            var practicalMaterial = await context.PracticalMaterials.FirstAsync(pm => pm.Id == practicalId);
            var tryCount = await context.TestResults
                .Where(tr => tr.PracticalMaterialId == practicalId && tr.UserId == userId).CountAsync();
            if (practicalMaterial.TriesCount == tryCount)
            {
                return BadRequest();
            }

            testRes = new TestResult { UserId = userId, PracticalMaterialId = practicalId, IsCompleted = false, TryNumber = tryCount + 1 };
            await context.TestResults.AddAsync(testRes);
            await context.SaveChangesAsync();
        }

        return Ok(new { testRes.TryNumber });
    }

    [HttpPost]
    public async Task<IActionResult> UploadTest([FromBody] CheckTestResultRequest request)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();
        var testResult = await context.TestResults.Include(tr => tr.PracticalMaterial).FirstOrDefaultAsync(tr => tr.PracticalMaterialId == request.PracticalId && tr.UserId == userId && !tr.IsCompleted);
        if (testResult is null)
        {
            return BadRequest();
        }

        testResult.IsCompleted = true;
        testResult.TurnedDate = DateTime.UtcNow;
        testResult.MaxScore = 0;
        testResult.Score = 0;

        List<Answer> answers = new List<Answer>();
        double score = 0, maxScore = 0;
        foreach (var ans in request.Answers)
        {
            var question = await context.Questions.FindAsync(ans.Id);
            var bindId = (await context.PracticalMaterialBindQuestions.FirstAsync(b => b.PracticalMaterialId == request.PracticalId && b.QuestionId == ans.Id)).Id;
            if (question is null)
            {
                continue;
            }

            maxScore += question.Weight;
            var result = ScoreHelper.GetAnswer((QuestionTypeEnum)question.QuestionTypeId, question, ans.Answer);
            score += result.QuestionScore;
            answers.Add(new Answer
            {
                PracticalMaterialBindQuestionId = bindId,
                Answers = JsonSerializer.Serialize(result),
                TestResultId = testResult.Id,
            });
        }

        testResult.MaxScore = maxScore;
        testResult.Score = score;
        await context.Answers.AddRangeAsync(answers);
        await context.SaveChangesAsync();


        return Ok(new
        {
            testResult.Id,
            testResult.UserId,
            testResult.Score,
            testResult.MaxScore,
            testResult.TryNumber,
            Grade = ScoreHelper.GetGrade(testResult.Score, testResult.MaxScore, testResult.PracticalMaterial.PercentForFive, testResult.PracticalMaterial.PercentForFour, testResult.PracticalMaterial.PercentForThree)
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskFile(long taskId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();
        var taskFile = await context.CaseFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(cf => cf.UserId == userId && cf.CaseId == taskId);
        if (taskFile is null)
        {
            return NotFound();
        }

        var comments = await context.CaseFileComments
            .AsNoTracking()
            .Where(cfc => cfc.CaseFileId == taskFile.Id && !cfc.IsGenerated)
            .OrderBy(cfc => cfc.Id)
            .Select(cfc => new { cfc.Id, cfc.Created, cfc.Text })
            .ToListAsync();

        var isUpdated = await context.CaseFileComments
            .AsNoTracking()
            .Where(cfc => cfc.CaseFileId == taskFile.Id)
            .OrderBy(cfc => cfc.Id)
            .Select(cfc => cfc.IsGenerated)
            .LastOrDefaultAsync();

        return Ok(new
        {
            taskFile.Path,
            taskFile.Id,
            Name = taskFile.Path.GetPublicFileName(),
            Comments = comments,
            taskFile.IsAccepted,
            IsUpdated = isUpdated,
            taskFile.Grade
        });
    }

    [HttpPut]
    public async Task<IActionResult> UploadTaskFile([FromForm] AddTaskFileRequest request)
    {
        // only one file per user
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();
        var taskFile = await context.CaseFiles
            .FirstOrDefaultAsync(cf => cf.UserId == userId && cf.CaseId == request.TaskId);

        if (taskFile is not null && taskFile.IsAccepted)
        {
            return BadRequest();
        }

        string comment;
        if (taskFile is not null)
        {
            FileHelper.DeleteFileFromPublic(taskFile.Path);
            var filePath = await FileHelper.SaveFileToPublic(request.File);
            taskFile.Path = filePath;
            comment = "Файл обновлен";
        }
        else
        {
            var filePath = await FileHelper.SaveFileToPublic(request.File);
            taskFile = new CaseFile() { CaseId = request.TaskId, UserId = userId, Path = filePath };
            await context.CaseFiles.AddAsync(taskFile);
            comment = "Файл добавлен";
        }

        await context.SaveChangesAsync();

        CaseFileComment taskFileComment = new()
        {
            IsGenerated = true,
            Text = comment,
            CaseFileId = taskFile.Id,
        };
        await context.CaseFileComments.AddAsync(taskFileComment);
        await context.SaveChangesAsync();

        return Ok(new
        {
            taskFile.Id,
            taskFile.Path,
            Name = taskFile.Path.GetPublicFileName(),
            taskFile.IsAccepted,
            IsUpdated = false
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetPracticals(long moduleId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();

        var result = await context.PracticalMaterials
            .AsNoTracking()
            .Include(pm => pm.PracticalBindUsers)
            .Where(pm => pm.ModuleId == moduleId && pm.IsPublic && pm.PracticalBindUsers.Any(pb => pb.UserId == userId))
            .Select(pm => new { pm.Id, pm.Name })
            .ToListAsync();
        return Ok(result);
    }


    [HttpGet]
    public async Task<IActionResult> GetTasks(long practicalId)
    {
        var login = User?.Identity?.Name;
        if (login is null)
        {
            return Unauthorized();
        }

        var userId = await context.Users.Where(u => u.Login == login).Select(u => u.Id).FirstOrDefaultAsync();

        var result = await context.Cases
            .Include(c => c.CaseFiles)
            .AsNoTracking()
            .Where(c => c.PracticalMaterialId == practicalId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Text,
                IsAccepted = c.CaseFiles.Any(cf => cf.UserId == userId && cf.IsAccepted),
                IsUpdated = c.CaseFiles.Any(cf => cf.UserId == userId && cf.Comments.OrderBy(cff => cff.Id).Last().IsGenerated),
                Grade = c.CaseFiles.Where(cf => cf.UserId == userId).Select(cf => cf.Grade).FirstOrDefault(),
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTheories(long moduleId)
    {
        var cannotAccess = await context.PracticalMaterials
            .AsNoTracking()
            .Include(pm => pm.TestResults)
            .AnyAsync(tm => tm.TestResults.Any(tr => !tr.IsCompleted));
        if (cannotAccess)
        {
            return Ok(new { CannotAccess = cannotAccess });
        }

        var result = await context.TheoreticalMaterials
            .AsNoTracking()
            .Where(m => m.ModuleId == moduleId)
            .Select(m => new { m.Id, m.Name })
            .ToListAsync();

        return Ok(new { Theories = result, CannotAccess = cannotAccess });
    }
}
