namespace Base.Tests.Interfaces;
public interface IBaseApplicationUnityTests
{
    Task GetAsync();
    Task ListAsync();
    Task AddAsync();
    Task UpdateAsync();
    Task DeleteAsync();
    Task LogicalDeleteAsync();
}
