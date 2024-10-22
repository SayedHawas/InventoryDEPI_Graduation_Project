using System.Text.RegularExpressions;

namespace smERP.Domain.ValueObjects;

public class File : ValueObject
{
    public string Path { get; } = null!;
    public string FileName { get; } = null!;
    public string ContentType { get; } = null!;

    protected File(string path, string fileName, string contentType)
    {
        Path = path;
        FileName = fileName;
        ContentType = contentType;
    }

    protected File() { }

    public static File Create(string path, string fileName, string contentType)
    {
        return new File(path, fileName, contentType);
    }

    public static File Create(string path)
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        string contentType = System.IO.Path.GetExtension(path)?.TrimStart('.') ?? string.Empty;

        return new File(path, fileName, contentType);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Path;
        yield return FileName;
        yield return ContentType;
    }

    protected static string? TryPrepareBase64(string? base64String, string mimeType)
    {
        if (string.IsNullOrWhiteSpace(base64String))
        {
            return null;
        }

        string base64Data = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

        if (!IsBase64String(base64Data))
        {
            return null;
        }

        string dataBeforeBase64 = base64String.Split(',')[0];
        if (!dataBeforeBase64.Contains(mimeType))
        {
            return null;
        }

        return base64Data;
    }

    private static bool IsBase64String(string base64)
    {
        base64 = base64.Trim();
        return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
    }

    protected static MemoryStream? ConvertBase64ToMemoryStream(string? base64String, string mimeType)
    {
        string? preparedBase64 = TryPrepareBase64(base64String, mimeType);

        if (preparedBase64 is null) return null;

        try
        {
            byte[] imageBytes = Convert.FromBase64String(preparedBase64);
            return new MemoryStream(imageBytes);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}