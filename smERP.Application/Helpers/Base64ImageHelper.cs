using System.Text.RegularExpressions;

namespace smERP.Application.Helpers;

public class Base64ImageHelper
{
    public string? TryPrepareBase64Image(string? base64String)
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
        if (!dataBeforeBase64.Contains("image/"))
        {
            return null;
        }

        return base64Data;
    }

    private bool IsBase64String(string base64)
    {
        base64 = base64.Trim();
        return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
    }

    public MemoryStream? ConvertBase64ToMemoryStream(string? preparedBase64)
    {
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