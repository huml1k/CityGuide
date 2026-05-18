using AdminService.Application.Dtos;

namespace AdminService.Application.Interfaces;

public interface IHealthCheckService
{
    Task<HealthCheckResponse> CheckAsync(CancellationToken cancellationToken = default);
}
