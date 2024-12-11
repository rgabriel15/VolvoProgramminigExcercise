using Base.Application.Interfaces.Validators;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.Validators;
public sealed class VehicleValidators : IBaseValidator<VehicleDto>
{
    #region Methods
    public bool IsValid(VehicleDto dto)
    {
        if (dto == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Color))
        {
            return false;
        }

        return true;
    }
    #endregion
}
