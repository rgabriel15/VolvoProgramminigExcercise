using Base.Tests.Features;
using VehicleType.Application.Interfaces.Services;
using VehicleType.Domain.Entities;
using VehicleType.Infrastructure.Repositories;

namespace VehicleType.Tests.Features;
public sealed class ArchitectureTests
    : BaseArchitectureTests<VehicleTypeEntity, VehicleTypeRepository, IVehicleTypeService>
{
}
