using System.Security.Cryptography.X509Certificates;
using Education.Models.Requests;

namespace Education.Helpers;

public static class FileHelper
{
    public static async Task<string> SaveFileToPublic(IFormFile file)
    {
        var dotIndex = file.FileName.LastIndexOf('.');
        var fileBase = file.FileName.Substring(0, dotIndex);
        var fileExt = file.FileName.Substring(dotIndex);
        var fileName = fileBase + "_" + Guid.NewGuid().ToString() + fileExt;
        var physName = Path.Combine("Files", fileName);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), physName);

        using FileStream fs = File.Create(filePath);
        await file.CopyToAsync(fs);
        return physName;
    }

    public static bool DeleteFileFromPublic(string name)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), name);
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
