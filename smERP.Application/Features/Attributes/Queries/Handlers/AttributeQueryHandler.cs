using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Attributes.Queries.Models;
using smERP.Application.Features.Attributes.Queries.Responses;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Queries.Handlers;

public class AttributeQueryHandler(IAttributeRepository attributeRepository) :
    IRequestHandler<GetAttributesQuery, IResult<IEnumerable<GetAttributesQueryResponse>>>,
    IRequestHandler<GetPaginatedAttributesQuery, IResult<PagedResult<GetAttributesQueryResponse>>>,
    IRequestHandler<GetAttributeQuery, IResult<GetAttributeQueryResponse>>
{
    private readonly IAttributeRepository _attributeRepository = attributeRepository;

    public async Task<IResult<IEnumerable<GetAttributesQueryResponse>>> Handle(GetAttributesQuery request, CancellationToken cancellationToken)
    {
        return new Result<IEnumerable<GetAttributesQueryResponse>>(await _attributeRepository.GetAttributesSelectionList());
    }

    public async Task<IResult<PagedResult<GetAttributesQueryResponse>>> Handle(GetPaginatedAttributesQuery request, CancellationToken cancellationToken)
    {
        var paginatedAttributes = await _attributeRepository.GetPagedAsync(request);
        return new Result<PagedResult<GetAttributesQueryResponse>>(paginatedAttributes);
    }

    public async Task<IResult<GetAttributeQueryResponse>> Handle(GetAttributeQuery request, CancellationToken cancellationToken)
    {
        var attribute = await _attributeRepository.GetByID(request.AttributeId);
        if (attribute == null)
            return new Result<GetAttributeQueryResponse>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Attribute.Localize()));

        var attributeResponse = new GetAttributeQueryResponse(attribute.Id, attribute.Name.English, attribute.Name.Arabic, attribute.AttributeValues.Select(value => new GetAttributeValuesQueryResponse(value.Id, value.Value.English, value.Value.Arabic)));
        return new Result<GetAttributeQueryResponse>(attributeResponse);
    }
}
