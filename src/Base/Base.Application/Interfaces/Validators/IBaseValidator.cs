namespace Base.Application.Interfaces.Validators;
public interface IBaseValidator<in T>
    where T : class
{
    bool IsValid(T dto);
}
