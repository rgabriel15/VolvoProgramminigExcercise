using Base.Tests.Features;
using ClientException.Application.Interfaces.Services;
using ClientException.Domain.Entities;
using ClientException.Infrastructure.Repositories;

namespace ClientException.Tests.Features;
public sealed class ArchitectureTests
    : BaseArchitectureTests<ClientExceptionEntity, ClientExceptionRepository, IClientExceptionService>
{
}
