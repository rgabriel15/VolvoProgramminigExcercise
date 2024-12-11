using Base.Application.Interfaces.Services;
using ClientException.Application.DTOs;

namespace ClientException.Application.Interfaces.Services;
public interface IClientExceptionService : IBaseService<ClientExceptionDto>
{
}
