using smERP.Application.Features.Attributes.Queries.Responses;
using smERP.SharedKernel.Responses;
using Attribute = smERP.Domain.Entities.Product.Attribute;

namespace smERP.Application.Contracts.Persistence;

public interface IAttributeRepository : IRepository<Attribute>
{
    Task<bool> DoesListExist(List<(int AttributeId, int AttributeValueId)> AttributeValuesIds);
    Task<IEnumerable<GetAttributesQueryResponse>> GetAttributesSelectionList();
    new Task<PagedResult<GetAttributesQueryResponse>> GetPagedAsync(PaginationParameters parameters);
}
