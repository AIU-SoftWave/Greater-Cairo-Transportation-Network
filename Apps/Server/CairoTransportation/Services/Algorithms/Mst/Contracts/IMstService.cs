using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Mst.DTOs;

namespace CairoTransportation.Services.Algorithms.Mst.Contracts;

public interface IMstService
{
    Task<AlgorithmResponseDto<MstResultDto>> BuildCheapestNetworkAsync();
}
