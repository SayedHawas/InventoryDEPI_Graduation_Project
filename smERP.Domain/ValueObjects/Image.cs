using ImageMagick;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using static System.Net.Mime.MediaTypeNames;

namespace smERP.Domain.ValueObjects;

public class Image : File
{
    private Image(string path, string fileName, string contentType)
    : base(path, fileName, contentType)
    {

    }

    private Image() { }

    public new static Image Create(string path)
    {
        string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        string contentType = System.IO.Path.GetExtension(path)?.TrimStart('.') ?? string.Empty;

        return new Image(path, fileName, contentType);
    }

    public static IResult<List<(MemoryStream MemoryStream, string ContentType)>> CreateMemoryStreams(List<string> base64Images)
    {
        var imageMemoryStreams = new List<(MemoryStream, string)>();

        foreach (var base64Image in base64Images)
        {
            // Remove the "data:image/jpeg;base64," or similar prefix if it exists
            var imageData = base64Image.Split(",")[1];

            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(imageData);
            }
            catch
            {
                return new Result<List<(MemoryStream MemoryStream, string ContentType)>>()
                    .WithBadRequestResult("One or more images have invalid Base64 format.");
            }

            // Validate the image format
            string? contentType = ValidateImageFormat(fileBytes);
            if (contentType == null)
                 return new Result<List<(MemoryStream MemoryStream, string ContentType)>>()
                    .WithBadRequestResult("Invalid image format. Supported formats are JPEG, PNG, and WEBP.");

            var memoryStreamResult = ImageMemoryStream(fileBytes);
            if (memoryStreamResult.IsFailed)
            {
                return new Result<List<(MemoryStream MemoryStream, string ContentType)>>()
                    .WithBadRequestResult("Error generating image stream.");
            }

            imageMemoryStreams.Add((memoryStreamResult.Value, contentType));
        }

        return new Result<List<(MemoryStream MemoryStream, string ContentType)>>(imageMemoryStreams);
    }

    private static string? ValidateImageFormat(byte[] fileBytes)
    {
        if (fileBytes.Length < 4)
            return null; // Not enough data to determine type

        // Check for magic numbers for supported image formats
        // PNG: 89 50 4E 47
        if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
            return "png";

        // JPEG: FF D8 FF
        if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF)
            return "jpeg";

        // WEBP: 52 49 46 46 (RIFF) + 57 45 42 50 (WEBP)
        if (fileBytes[0] == 0x52 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x46 &&
            fileBytes[8] == 0x57 && fileBytes[9] == 0x45 && fileBytes[10] == 0x42 && fileBytes[11] == 0x50)
            return "webp";

        return null; // Unknown file type
    }

    //public static IResultBase GenerateImageStream(string base64String)
    //{
    //    var memoryStream = ConvertBase64ToMemoryStream(base64String, "/image");
    //    if (memoryStream == null)
    //        return new Result<Image>()
    //            .WithBadRequestResult(SharedResourcesKeys.ThereSomeThingWrongWithImageProvided.Localize());

    //    return ImageMemoryStream(memoryStream);
    //}

    private static IResult<MemoryStream> ImageMemoryStream(byte[] fileBytes)
    {
        try
        {
            using var image = new MagickImage(fileBytes);
            var memoryStream = new MemoryStream();
            image.Write(memoryStream);

            memoryStream.Position = 0;

            return new Result<MemoryStream>(memoryStream);
        }
        catch
        {
            return new Result<MemoryStream>()
                .WithBadRequestResult(SharedResourcesKeys.ThereSomeThingWrongWithImageProvided.Localize());
        }
    }
}
