using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using File = smERP.Domain.ValueObjects.File;

namespace smERP.Persistence.Data.Converters;

public class FileValueConverter : ValueConverter<File, string>
{
    public FileValueConverter()
        : base(
            file => file.Path,
            path => File.Create(path))
    {
    }
}