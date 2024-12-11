namespace Base.Tests.Interfaces;
public interface IBaseInfrastructureUnityTests
{
    Task GetAsync();
    Task ListAsync();
    Task AddAsync();
    Task UpdateAsync();
    Task DeleteAsync();
}
