using smERP.SharedKernel.Enums;
using System.Security.Cryptography;
using System.Text;

namespace smERP.Persistence.Managers;

public class FileStorageManager
{
    private const int FilesPerFolder = 1000;
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly Dictionary<FileType, string> _fileTypePaths;

    public FileStorageManager(string basePath, string baseUrl = "https://smerp.runasp.net/")
    {
        _basePath = basePath;
        _baseUrl = baseUrl;
        _fileTypePaths = new Dictionary<FileType, string>
        {
            { FileType.ProductImage, "products\\images" },
            { FileType.UserAvatar, "users\\avatars" },
            { FileType.Document, "documents" },
            { FileType.Miscellaneous, "misc" },
            { FileType.CompanyCover, "company\\cover" },
            { FileType.CompanyIcon, "company\\icon" }
        };
    }

    public string GetStoragePath(FileType fileType, string fileName)
    {
        var fileTypeBasePath = GetFileTypeBasePath(fileType);
        var fileHash = GetFileHash(fileName);
        var folderStructure = CreateFolderStructure(fileHash);
        var fullPath = Path.Combine(_basePath, fileTypeBasePath, folderStructure, fileName);
        EnsureDirectoryExists(Path.GetDirectoryName(fullPath));
        return fullPath;
    }

    private string GetFileTypeBasePath(FileType fileType)
    {
        if (_fileTypePaths.TryGetValue(fileType, out var path))
        {
            return path;
        }
        return _fileTypePaths[FileType.Miscellaneous];
    }

    private string GetFileHash(string fileName)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(fileName);
        byte[] hashBytes = MD5.HashData(inputBytes);
        return Convert.ToHexString(hashBytes);
    }

    private string CreateFolderStructure(string fileHash)
    {
        var folderName = Convert.ToInt32(fileHash.Substring(0, 4), 16) / FilesPerFolder;
        var subFolderName = Convert.ToInt32(fileHash.Substring(4, 4), 16) / FilesPerFolder;
        return Path.Combine(folderName.ToString("D3"), subFolderName.ToString("D3"));
    }

    public void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public string ConvertToWebUrl(string filePath)
    {
        var webRootIndex = filePath.IndexOf("FileStorage");
        if (webRootIndex == -1)
        {
            return filePath;
        }
        var webPath = filePath.Substring(webRootIndex);
        var relativePath = webPath.Replace('\\', '/');
        return $"{_baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    public string ConvertUrlToFilePath(string url)
    {
        if (!url.StartsWith(_baseUrl))
        {
            throw new ArgumentException("The provided URL does not match the expected base URL.", nameof(url));
        }

        var relativePath = url.Substring(_baseUrl.Length).TrimStart('/');

        if (relativePath.StartsWith("FileStorage/", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Substring("FileStorage/".Length);
        }

        var filePath = Path.Combine(_basePath, relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The file does not exist at the computed path.", filePath);
        }

        return filePath;
    }

    public void AddFileType(FileType fileType, string path)
    {
        if (!_fileTypePaths.ContainsKey(fileType))
        {
            _fileTypePaths.Add(fileType, path);
        }
        else
        {
            throw new ArgumentException($"File type {fileType} already exists.");
        }
    }

    public bool RemoveFileType(FileType fileType)
    {
        return _fileTypePaths.Remove(fileType);
    }

    public IReadOnlyDictionary<FileType, string> GetFileTypePaths()
    {
        return _fileTypePaths;
    }
}