using System.Diagnostics;
using System.Text.Json;
using Web.UI.Services.Base.Service.Models;
using Web.UI.Services.Base.Service.Services;
using Web.UI.Services.ClientException.Service.Interfaces;
using Web.UI.Services.ClientException.Service.Models;

namespace Web.UI.Services.ClientException.Service.Services;
internal sealed class ClientExceptionService : IClientExceptionService
{
    #region Constants
    private const string BaseUrl = "clientException";
    private readonly IHttpClientFactory HttpClientFactory;
    #endregion

    #region Constructors
    public ClientExceptionService(IHttpClientFactory httpClientFactory)
    {
        HttpClientFactory = httpClientFactory;
    }
    #endregion

    #region Methods
    public async Task<ClientExceptionModel> AddAsync(ClientExceptionModel model)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("VolvoProgrammingExerciseClientV1");
            using var cancellationTokenSource = new CancellationTokenSource(BaseService<BaseModel>.Timeout);
            var res = await httpClient.PostAsJsonAsync(
                requestUri: BaseUrl
                , value: model
                , cancellationToken: cancellationTokenSource.Token);

            if (!res.IsSuccessStatusCode)
            {
                return Activator.CreateInstance<ClientExceptionModel>();
            }

            var jsonStream = await res.Content.ReadAsStreamAsync();
            model = (await JsonSerializer.DeserializeAsync<ClientExceptionModel>(
                utf8Json: jsonStream
                , options: BaseService<BaseModel>.JsonSerializerOptions
                , cancellationToken: cancellationTokenSource.Token))
                ?? Activator.CreateInstance<ClientExceptionModel>();

            return model!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return Activator.CreateInstance<ClientExceptionModel>();
        }
    }
    #endregion
}
