using smERP.Application.Features.Suppliers.Commands.Models;

namespace smERP.Application.Features.Suppliers.Queries.Responses;

public record GetPaginatedSuppliersQueryResponse(
    int SupplierId,
    string Name,
    IEnumerable<Address> Addresses,
    IEnumerable<SuppliedProduct> Products);

public record SuppliedProduct(int ProductId, string Name, DateTime FirstTime, DateTime LastTime);