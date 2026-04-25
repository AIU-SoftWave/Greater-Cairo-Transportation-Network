using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;

namespace CairoTransportation.Services.Algorithms.MaintenancePlanning.Contracts;

public interface IMaintenancePlanningService
{
    Task<AlgorithmResponseDto<MaintenancePlanningResultDto>> GenerateMaintenancePlanAsync(double budget);
}

