using Microsoft.AspNetCore.Mvc;
using smERP.SharedKernel.Bases;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.Domain.Entities.Organization;
using smERP.Domain.ValueObjects;
using smERP.Domain.Entities.Product;

namespace smERP.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        //[HttpGet]
        //public async Task<IActionResult> Test(Product name)
        //{
        //    var g = System.Globalization.CultureInfo.CurrentCulture;
        //    var l = Result.From(() => Company.Create(g.DisplayName));
        //    l.AddError(Error.Create("test test"));
        //    var h = SharedResourcesKeys.Required_FieldName.Localize("Company name");
        //    return Ok(l);
        //}
    }
}
