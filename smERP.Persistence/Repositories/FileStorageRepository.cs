using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Managers;
using smERP.SharedKernel.Enums;

namespace smERP.Persistence.Repositories;

public class FileStorageRepository : IFileStorageRepository
{
    private readonly FileStorageManager _storageManager;

    public FileStorageRepository(FileStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    public async Task<string?> StoreFile(Stream fileStream, FileType fileType, string fullName, CancellationToken cancellationToken = default)
    {
        var filePath = _storageManager.GetStoragePath(fileType, fullName);
        try
        {
            using (var destinationStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await fileStream.CopyToAsync(destinationStream, 4096, cancellationToken);
            }
            return _storageManager.ConvertToWebUrl(filePath);
        }
        catch
        {
            await DeleteFile(_storageManager.ConvertToWebUrl(filePath), cancellationToken);
            return null;
        }
    }

    private string ConvertToWebUrl(string filePath)
    {
        var webRootIndex = filePath.IndexOf("FileStorage");
        if (webRootIndex == -1)
        {
            return filePath;
        }

        var webPath = filePath.Substring(webRootIndex);
        var relativePath = webPath.Replace('\\', '/');

        var baseUrl = "http://localhost:5184";
        return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    public Task<bool> DeleteFile(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = _storageManager.ConvertUrlToFilePath(url);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
