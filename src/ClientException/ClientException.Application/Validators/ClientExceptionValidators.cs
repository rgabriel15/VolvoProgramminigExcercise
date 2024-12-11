using Base.Application.Interfaces.Validators;
using ClientException.Application.DTOs;

namespace ClientException.Application.Validators;
public sealed class ClientExceptionValidators : IBaseValidator<ClientExceptionDto>
{
    #region Methods
    public bool IsValid(ClientExceptionDto dto)
    {
        if (dto == null)
        {
            return false;
        }

        return true;
    }
    #endregion
}
