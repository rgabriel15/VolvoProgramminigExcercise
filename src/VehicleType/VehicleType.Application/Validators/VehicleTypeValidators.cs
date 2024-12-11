using Base.Application.Interfaces.Validators;
using VehicleType.Application.DTOs;

namespace VehicleType.Application.Validators;
public sealed class VehicleTypeValidators : IBaseValidator<VehicleTypeDto>
{
    #region Methods
    public bool IsValid(VehicleTypeDto dto)
    {
        if (dto == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return false;
        }

        if (dto.NumberOfPassengers < 1)
        {
            return false;
        }

        return true;
    }
    #endregion
}
