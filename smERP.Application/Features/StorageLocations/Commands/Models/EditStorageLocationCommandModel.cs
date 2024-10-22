using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.StorageLocations.Commands.Models;

public record EditStorageLocationCommandModel(int BranchId, int StorageLocationId, string Name) : IRequest<IResultBase>;