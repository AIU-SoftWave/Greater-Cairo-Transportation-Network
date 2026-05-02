using System.Text.Json;
using System.Text.Json.Serialization;

namespace CairoTransportation.Modules.TrafficControl.Services;

public class MlPrediction
{
    [JsonPropertyName("road_id")]
    public long RoadId { get; set; }
    
    [JsonPropertyName("period")]
    public string Period { get; set; } = string.Empty;
    
    [JsonPropertyName("predicted_congestion")]
    public double PredictedCongestion { get; set; }
}

public interface IMlPredictionService
{
    Task<Dictionary<(long RoadId, string Period), double>> GetCongestionMapAsync();
    Task<List<MlPrediction>> GetAllPredictionsAsync();
    Task<double?> GetCongestionAsync(long roadId, string period);
}

public class MlPredictionService : IMlPredictionService
{
    private readonly List<MlPrediction> _predictions;
    private readonly Dictionary<(long RoadId, string Period), double> _congestionMap;

    public MlPredictionService()
    {
        _predictions = LoadPredictions();
        _congestionMap = _predictions
            .GroupBy(p => (p.RoadId, Period: p.Period.Trim().ToUpperInvariant()))
            .ToDictionary(g => g.Key, g => g.First().PredictedCongestion);
    }

    private static List<MlPrediction> LoadPredictions()
    {
        string basePath = AppContext.BaseDirectory;
        string filePath = Path.Combine(basePath, "Data", "predictions.json");

        if (!File.Exists(filePath))
        {
            return [];
        }

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<MlPrediction>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }

    public Task<Dictionary<(long RoadId, string Period), double>> GetCongestionMapAsync() =>
        Task.FromResult(_congestionMap);

    public Task<List<MlPrediction>> GetAllPredictionsAsync() =>
        Task.FromResult(_predictions);

    public Task<double?> GetCongestionAsync(long roadId, string period)
    {
        string normalizedPeriod = period.Trim().ToUpperInvariant();
        bool found = _congestionMap.TryGetValue((roadId, normalizedPeriod), out double congestion);
        return Task.FromResult(found ? congestion : (double?)null);
    }
}
