using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Commands.Models;

public record DeleteBranchCommandModel(int BranchId) : IRequest<IResultBase>;