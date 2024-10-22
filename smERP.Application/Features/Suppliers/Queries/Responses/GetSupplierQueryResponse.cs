using smERP.Application.Features.Suppliers.Commands.Models;

namespace smERP.Application.Features.Suppliers.Queries.Responses;

public record GetSupplierQueryResponse(
    int SupplierId,
    string EnglishName,
    string ArabicName,
    IEnumerable<Address> Addresses
    );