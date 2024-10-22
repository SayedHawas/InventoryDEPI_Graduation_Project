using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProductInstances.Commands.Models;

public record AddProductInstanceCommandModel(
    int ProductId,
    decimal? SellingPrice,
    List<string>? ImagesBase64,
    List<ProductInstanceAttributeValue> Attributes) : IRequest<IResultBase>;

public record ProductInstanceAttributeValue(
    int AttributeId,
    int AttributeValueId);