using MediatR;
using smERP.Application.MarkerInterfaces;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Categories.Commands.Models;

public record AddCategoryCommandModel(string ArabicName, string EnglishName,int? ParentCategoryId) : IRequest<IResultBase>;
