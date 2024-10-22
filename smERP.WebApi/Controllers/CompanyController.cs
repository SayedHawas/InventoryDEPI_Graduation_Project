//using Microsoft.AspNetCore.Mvc;
//using smERP.Application.Features.Companies.Commands.Models;
//using smERP.Domain.Entities.Organization;
//using smERP.SharedKernel.Bases;

//namespace smERP.WebApi.Controllers;

//[Route("[controller]")]
//[ApiController]
//public class CompanyController : AppControllerBase
//{
//    [HttpPost]
//    public async Task<IActionResult> Create([FromBody] AddCompanyCommandModel request)
//    {
//        var response = await Mediator.Send(request);
//        var apiResult = response.ToApiResult();
//        return StatusCode(apiResult.StatusCode, apiResult);
//    }
//}
