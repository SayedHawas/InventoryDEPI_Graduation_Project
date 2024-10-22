using Microsoft.AspNetCore.Mvc;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Categories.Commands.Models;
using smERP.Application.Features.ProcurementTransactions.Commands.Models;
using smERP.Application.Features.ProcurementTransactions.Queries.Models;

namespace smERP.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ProcurementTransactionsController : AppControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddProcurementTransactionCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EditProcurementTransactionCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedProcurementTransactions([FromQuery] GetPaginatedProcurementTransactionQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("{transactionId:int}/payments/{paymentId:int}")]
    public async Task<IActionResult> GetPayment(int transactionId, int paymentId)
    {
        var response = await Mediator.Send(new GetProcurementTransactionPaymentQuery(transactionId, paymentId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost("payments")]
    public async Task<IActionResult> AddPayment([FromBody] AddProcurementTransactionPaymentCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("payments")]
    public async Task<IActionResult> UpdatePayment([FromBody] EditProcurementTransactionPaymentCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpDelete("{transactionId:int}/payments/{paymentId:int}")]
    public async Task<IActionResult> RemovePayment(int transactionId, int paymentId)
    {
        var response = await Mediator.Send(new DeleteProcurementTransactionPaymentCommandModel(transactionId, paymentId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("{transactionId:int}/products/{productInstanceId:int}")]
    public async Task<IActionResult> GetProduct(int transactionId, int productInstanceId)
    {
        var response = await Mediator.Send(new GetProcurementTransactionProductQuery(transactionId, productInstanceId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromBody] AddProcurementTransactionProductCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("products")]
    public async Task<IActionResult> UpdateProduct([FromBody] EditProcurementTransactionProductCommandModel request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpDelete("{transactionId:int}/products/{productInstanceId:int}")]
    public async Task<IActionResult> RemoveProduct(int transactionId, int productInstanceId)
    {
        var response = await Mediator.Send(new DeleteProcurementTransactionProductCommandModel(transactionId, productInstanceId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }
}
