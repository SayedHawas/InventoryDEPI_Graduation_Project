using MediatR;
using smERP.Application.MarkerInterfaces;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Companies.Commands.Models;

public record AddCompanyCommandModel(string CompanyName) : IRequest<IResultBase>, ITransactionalRequest;
