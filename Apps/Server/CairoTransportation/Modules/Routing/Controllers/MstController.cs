using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Mst.Contracts;
using CairoTransportation.Services.Algorithms.Mst.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/network-expansion")]
public class MstController(IMstService mstService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCheapestNetwork()
    {
        AlgorithmResponseDto<MstResultDto> result = await mstService.BuildCheapestNetworkAsync();
        return result.Success ? Ok(result) : NotFound(result);
    }
}

