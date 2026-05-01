using CairoTransportation.Modules.Routing.Services.Contracts;
using CairoTransportation.Utils.Helpers.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.Routing.Controllers;

[ApiController]
[Route("api/network-expansion")]
public class MstController(INetworkExpansionService networkExpansionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCheapestNetwork()
    {
        AlgorithmResponseDto<MstResultDto> result = await networkExpansionService.BuildCheapestNetworkAsync();
        return result.Success ? Ok(result) : NotFound(result);
    }
}

