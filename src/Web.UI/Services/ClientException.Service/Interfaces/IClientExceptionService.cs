using Web.UI.Services.ClientException.Service.Models;

namespace Web.UI.Services.ClientException.Service.Interfaces;
internal interface IClientExceptionService
{
    Task<ClientExceptionModel> AddAsync(ClientExceptionModel model);
}
