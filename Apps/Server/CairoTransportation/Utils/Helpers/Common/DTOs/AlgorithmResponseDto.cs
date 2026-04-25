namespace CairoTransportation.Services.Algorithms.Common.DTOs;

public class AlgorithmResponseDto<TData>
{
    public string AlgorithmName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
    public AlgorithmTraceDto Trace { get; set; } = new();
    public TData? Data { get; set; }
}

