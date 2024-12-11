using Base.Application.Interfaces.Validators;
using Chassis.Application.DTOs;

namespace Chassis.Application.Validators;
public sealed class ChassisValidators : IBaseValidator<ChassisDto>
{
    #region Methods
    public bool IsValid(ChassisDto dto)
    {
        if (dto == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.ChassisSeries))
        {
            return false;
        }

        return true;
    }
    #endregion
}
