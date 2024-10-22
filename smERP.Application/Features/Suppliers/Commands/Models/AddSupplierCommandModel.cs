using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Commands.Models;

public record AddSupplierCommandModel(
    string EnglishName,
    string ArabicName,
    List<Address> Addresses) 
    : IRequest<IResultBase>;

public record Address(
    string Street, 
    string City, 
    string State, 
    string Country, 
    string PostalCode, 
    string Comment);