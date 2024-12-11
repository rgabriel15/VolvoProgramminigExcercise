using Base.Tests.Features;
using Chassis.Application.Interfaces.Services;
using Chassis.Domain.Entities;
using Chassis.Infrastructure.Repositories;

namespace Chassis.Tests.Features;
public sealed class ArchitectureTests
    : BaseArchitectureTests<ChassisEntity, ChassisRepository, IChassisService>
{
}
