using Microsoft.AspNetCore.Mvc;
using smERP.Application.Features.Attributes.Commands.Models;
using smERP.Application.Features.Attributes.Queries.Models;
using smERP.SharedKernel.Responses;

namespace smERP.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AttributesController : AppControllerBase
{

    [HttpGet("List")]
    public async Task<IActionResult> GetAttributes()
    {
        var response = await Mediator.Send(new GetAttributesQuery());
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedAttributes([FromQuery] GetPaginatedAttributesQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("{attributeId:int}")]
    public async Task<IActionResult> GetAttribute(int attributeId)
    {
        var response = await Mediator.Send(new GetAttributeQuery(attributeId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAttribute([FromBody] AddAttributeCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAttribute([FromBody] EditAttributeCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAttribute([FromBody] DeleteAttributeCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost("{request.AttributeId}/values")]
    public async Task<IActionResult> CreateAttributeValue([FromBody] AddAttributeValueCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("{request.AttributeId}/values/{request.AttributeValueId}")]
    public async Task<IActionResult> UpdateAttributeValue([FromBody] EditAttributeValueCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpDelete("{request.AttributeId}/values/{request.AttributeValueId}")]
    public async Task<IActionResult> DeleteAttributeValue([FromBody] DeleteAttributeValueCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }
}
