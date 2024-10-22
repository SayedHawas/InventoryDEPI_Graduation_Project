using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProductInstances.Commands.Models;

public record EditProductInstanceCommandModel : IRequest<IResultBase>
{
    public int ProductId { get; set; }
    public int ProductInstanceId { get; set; }
    public decimal? SellingPrice { get; init; }
    public List<string>? ImagesPathToRemove { get; init; }
    public List<string>? ImagesBase64 { get; init; }
    public List<ProductInstanceAttributeValue> Attributes { get; init; } = new();

    public EditProductInstanceCommandModel(decimal? sellingPrice = null, List<ProductInstanceAttributeValue>? productInstanceAttributeValues = null)
    {
        SellingPrice = sellingPrice;
        if (productInstanceAttributeValues != null)
        {
            Attributes = productInstanceAttributeValues;
        }
    }

    public EditProductInstanceCommandModel() { }
}