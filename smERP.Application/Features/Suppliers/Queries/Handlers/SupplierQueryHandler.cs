using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Suppliers.Commands.Models;
using smERP.Application.Features.Suppliers.Queries.Models;
using smERP.Application.Features.Suppliers.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Queries.Handlers;

public class SupplierQueryHandler(ISupplierRepository supplierRepository) :
    IRequestHandler<GetPaginatedSuppliersQuery, IResult<PagedResult<GetPaginatedSuppliersQueryResponse>>>,
    IRequestHandler<GetSuppliersQuery, IResult<IEnumerable<SelectOption>>>,
    IRequestHandler<GetSupplierQuery, IResult<GetSupplierQueryResponse>>

{
    ISupplierRepository _supplierRepository = supplierRepository;

    public async Task<IResult<PagedResult<GetPaginatedSuppliersQueryResponse>>> Handle(GetPaginatedSuppliersQuery request, CancellationToken cancellationToken)
    {
        var paginatedSuppliers = await _supplierRepository.GetPagedAsync(request);
        return new Result<PagedResult<GetPaginatedSuppliersQueryResponse>>(paginatedSuppliers);
    }

    public async Task<IResult<IEnumerable<SelectOption>>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        return new Result<IEnumerable<SelectOption>>(await _supplierRepository.GetSuppliers());
    }

    public async Task<IResult<GetSupplierQueryResponse>> Handle(GetSupplierQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByID(request.SupplierId);
        if (supplier == null)
            return new Result<GetSupplierQueryResponse>().WithNotFound();

        var response = new GetSupplierQueryResponse(supplier.Id, supplier.Name.English, supplier.Name.Arabic, supplier.Addresses.Select(address => new Address(address.Street, address.City, address.State, address.Country, address.PostalCode, address.Comment ?? "")));
        return new Result<GetSupplierQueryResponse>(response);
    }
}
