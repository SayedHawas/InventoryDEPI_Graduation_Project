using Microsoft.AspNetCore.Mvc;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Brands.Commands.Models;
using smERP.Application.Features.Brands.Queries.Models;

namespace smERP.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class BrandsController : AppControllerBase
{
    [HttpGet("List")]
    public async Task<IActionResult> GetBrands()
    {
        var response = await Mediator.Send(new GetBrandsQuery());
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddBrandCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EditBrandCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBrandCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedBrands([FromQuery] GetPaginatedBrandsQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("{brandId:int}")]
    public async Task<IActionResult> GetBrandById(int brandId)
    {
        var response = await Mediator.Send(new GetBrandQuery(brandId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }
}
