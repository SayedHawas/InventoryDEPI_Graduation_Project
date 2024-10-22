using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.StorageLocations.Commands.Models;

public record AddStorageLocationCommandModel(int BranchId, string Name) : IRequest<IResultBase>;
