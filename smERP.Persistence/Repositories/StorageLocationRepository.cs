using smERP.Domain.Entities.Organization;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;

namespace smERP.Persistence.Repositories;

public class StorageLocationRepository(ProductDbContext context) : Repository<StorageLocation>(context), IStorageLocationRepository
{
}
