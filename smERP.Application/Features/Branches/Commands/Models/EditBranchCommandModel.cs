using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Commands.Models;

public record EditBranchCommandModel(int BranchId, string? EnglishName, string? ArabicName) : IRequest<IResultBase>;