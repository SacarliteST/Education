using Education.DAL.Models;

namespace Education.Extensions;

public static class StringExtensions
{
    public static string GetPublicFileName(this string str)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(str, nameof(str));
        var dotInd = str.LastIndexOf('.');
        var fileExt = str.Substring(dotInd);
        var fileBase = str.Substring(0, dotInd);
        var undInd = fileBase.LastIndexOf('_');
        var slashInd = fileBase.IndexOf(Path.DirectorySeparatorChar);
        var fileName = fileBase.Substring(slashInd + 1, undInd - slashInd - 1);
        return fileName + fileExt;
    }

    public static string GetFullName(this User user)
    {
        return $"{user.LastName} {user.FirstName} {user.MiddleName}";
    }
}
