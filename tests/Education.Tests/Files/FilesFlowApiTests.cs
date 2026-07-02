using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.TaskFiles;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.Files;

public sealed class FilesFlowApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public FilesFlowApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task UploadTaskFile_SavesMetadataAndPhysicalFile()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await UploadTaskFileAsync(client, "solution.txt", "student solution");

        response.EnsureSuccessStatusCode();
        var taskFile = await response.Content.ReadFromJsonAsync<TaskFileResponse>();

        Assert.NotNull(taskFile);
        Assert.Equal("solution.txt", taskFile!.Name);
        Assert.True(File.Exists(Path.Combine(factory.FileStorageRoot, taskFile.StorageKey)));
        Assert.NotEqual("solution.txt", taskFile.StorageKey);
    }

    [Fact]
    public async Task Student_CannotDownloadAnotherStudentTaskFile()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.GetAsync('/' + ApiRoutes.Files.ForDownload("other-student.txt"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_DownloadsOnlyOwnCourseTaskFiles()
    {
        var studentClient = factory.CreateClient();
        studentClient.AuthenticateAs(EducationRoles.Student);
        var uploadResponse = await UploadTaskFileAsync(studentClient, "teacher-visible.txt", "visible for teacher");
        var uploaded = await uploadResponse.Content.ReadFromJsonAsync<TaskFileResponse>();

        var teacherClient = factory.CreateClient();
        teacherClient.AuthenticateAs(EducationRoles.Teacher);

        var ownFileResponse = await teacherClient.GetAsync('/' + ApiRoutes.Files.ForDownload(uploaded!.StorageKey));
        var otherFileResponse = await teacherClient.GetAsync('/' + ApiRoutes.Files.ForDownload("other-teacher.txt"));

        ownFileResponse.EnsureSuccessStatusCode();
        Assert.Equal("visible for teacher", await ownFileResponse.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Forbidden, otherFileResponse.StatusCode);
    }

    [Fact]
    public async Task Download_RejectsPathTraversal()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.GetAsync("/" + ApiRoutes.PrefixV1 + "/files/..%5Csecret.txt");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<HttpResponseMessage> UploadTaskFileAsync(HttpClient client, string fileName, string content)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(content), "file", fileName);

        return await client.PutAsync('/' + ApiRoutes.TaskFiles.ForStudentTaskFile(1), form);
    }
}
