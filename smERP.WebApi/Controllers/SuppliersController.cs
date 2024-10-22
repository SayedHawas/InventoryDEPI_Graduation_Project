using Microsoft.AspNetCore.Mvc;
using smERP.Application.Features.Brands.Queries.Models;
using smERP.Application.Features.Suppliers.Commands.Models;
using smERP.Application.Features.Suppliers.Queries.Models;
using smERP.Domain.Entities.ExternalEntities;

namespace smERP.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class SuppliersController : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaginatedSuppliers([FromQuery] GetPaginatedSuppliersQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("List")]
    public async Task<IActionResult> GetSuppliers()
    {
        var response = await Mediator.Send(new GetSuppliersQuery());
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("{supplierId:int}")]
    public async Task<IActionResult> GetSupplier(int supplierId)
    {
        var response = await Mediator.Send(new GetSupplierQuery(supplierId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddSupplierCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("{supplierId:int}")]
    public async Task<IActionResult> Update(int supplierId, [FromBody] EditSupplierCommandModel request)
    {
        request.SupplierId = supplierId;
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteSupplierCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }
}
