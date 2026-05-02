using CairoTransportation.Modules.TrafficControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.TrafficControl.Controllers;

[ApiController]
[Route("api/ml-predictions")]
public class MlPredictionsController(IMlPredictionService predictionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<MlPrediction> predictions = await predictionService.GetAllPredictionsAsync();
        return Ok(predictions);
    }

    [HttpGet("{roadId}/{period}")]
    public async Task<IActionResult> GetByRoadAndPeriod(long roadId, string period)
    {
        double? congestion = await predictionService.GetCongestionAsync(roadId, period);
        if (congestion == null)
        {
            return NotFound($"No prediction found for road {roadId} and period {period}.");
        }
        return Ok(new { roadId, period, predictedCongestion = congestion });
    }
}