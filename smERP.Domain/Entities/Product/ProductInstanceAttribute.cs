using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.Entities.Product;

public class ProductInstanceAttribute : Entity
{
    public int ProductInstanceId { get; private set; }
    public int AttributeValueId { get; private set; }
    public virtual AttributeValue AttributeValue { get; private set; } = null!;
    public virtual ProductInstance ProductInstance { get; private set; } = null!;

    private ProductInstanceAttribute(int productInstanceId, int attributeValueId)
    {
        ProductInstanceId = productInstanceId;
        AttributeValueId = attributeValueId;
    }

    internal ProductInstanceAttribute() { }

    public static IResult<ProductInstanceAttribute> Create(int productInstanceId, int attributeValueId)
    {
        var attribute = new ProductInstanceAttribute(productInstanceId, attributeValueId);
        return new Result<ProductInstanceAttribute>(attribute)
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public IResult<ProductInstanceAttribute> UpdateAttributeValue(int newAttributeValueId)
    {
        AttributeValueId = newAttributeValueId;
        return new Result<ProductInstanceAttribute>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }
}
