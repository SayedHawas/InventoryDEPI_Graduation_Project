using smERP.SharedKernel.Enums;

namespace smERP.Application.Contracts.Persistence;

public interface IFileStorageRepository
{
    Task<string?> StoreFile(Stream fileStream, FileType fileType, string fullName, CancellationToken cancellationToken = default);
    Task<bool> DeleteFile(string filePath, CancellationToken cancellationToken = default);
}