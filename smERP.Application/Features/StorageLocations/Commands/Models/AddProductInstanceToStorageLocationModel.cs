using MediatR;
using smERP.Application.Features.ProcurementTransactions.Commands.Models;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.StorageLocations.Commands.Models;

public record AddProductInstanceToStorageLocationModel(int StorageLocationId, int ProcurementTransactionId, List<ProductEntry> Products) : IRequest<IResultBase>;