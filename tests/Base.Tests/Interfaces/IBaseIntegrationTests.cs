namespace Base.Tests.Interfaces;
public interface IBaseIntegrationTests
{
    Task GetAsync();
    Task ListAsync();
    Task PostAsync();
    Task UpdateAsync();
    Task DeleteAsync();
    Task LogicalDeleteAsync();
}
