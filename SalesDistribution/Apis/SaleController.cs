using Microsoft.AspNetCore.Mvc;
using static SalesDistribution.SaleCommon;

namespace SalesDistribution.Apis;

[Route("api/[controller]/[action]")]
[ApiController]
public class SaleController : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    public Sale Calc([FromForm] Sale sale)
    {
        return SaleCommon.Calc(sale);
    }
}
