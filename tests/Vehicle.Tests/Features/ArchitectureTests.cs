using Base.Tests.Features;
using Vehicle.Application.Interfaces.Services;
using Vehicle.Domain.Entities;
using Vehicle.Infrastructure.Repositories;

namespace Vehicle.Tests.Features;
public sealed class ArchitectureTests
    : BaseArchitectureTests<VehicleEntity, VehicleRepository, IVehicleService>
{
}
