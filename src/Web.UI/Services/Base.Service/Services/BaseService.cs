using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Web.UI.Services.Base.Service.Interfaces;
using Web.UI.Services.Base.Service.Models;
using Web.UI.Services.ClientException.Service.Interfaces;
using Web.UI.Services.ClientException.Service.Models;

namespace Web.UI.Services.Base.Service.Services;
internal abstract class BaseService<T> : IBaseService<T>
    where T : BaseModel
{
    #region Constants
    internal static readonly TimeSpan Timeout =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? TimeSpan.FromHours(1)
            : TimeSpan.FromSeconds(60);
    public readonly IHttpClientFactory HttpClientFactory;
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
    public IHttpContextAccessor HttpContextAccessor;
    protected readonly IClientExceptionService ClientExceptionService;
    private readonly string BaseUrl;
    #endregion

    #region Constructors
    private protected BaseService(IHttpClientFactory httpClientFactory
        , IHttpContextAccessor httpContextAccessor
        , IClientExceptionService clientExceptionService
        , string baseUrlAddress)
    {
        HttpClientFactory = httpClientFactory;
        HttpContextAccessor = httpContextAccessor;
        ClientExceptionService = clientExceptionService;
        BaseUrl = !string.IsNullOrWhiteSpace(baseUrlAddress)
            ? baseUrlAddress
            : throw new ArgumentException(null, nameof(baseUrlAddress));
    }
    #endregion

    #region Methods
    public virtual async Task<T> AddAsync(T model)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("VolvoProgrammingExerciseClientV1");
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);
            var res = await httpClient.PostAsJsonAsync(
                requestUri: BaseUrl
                , value: model
                , cancellationToken: cancellationTokenSource.Token);

            if (!res.IsSuccessStatusCode)
            {
                return Activator.CreateInstance<T>();
            }

            var jsonStream = await res.Content.ReadAsStreamAsync();
            model = (await JsonSerializer.DeserializeAsync<T>(
                utf8Json: jsonStream
                , options: JsonSerializerOptions
                , cancellationToken: cancellationTokenSource.Token))
                ?? Activator.CreateInstance<T>();

            return model!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            var clientException = new ClientExceptionModel
            {
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty
            };
            _ = await ClientExceptionService.AddAsync(clientException);
            return Activator.CreateInstance<T>();
        }
    }

    public virtual async Task<ulong> DeleteAsync(ulong id)
    {
        try
        {
            var url = $"{BaseUrl}?id={id}";
            var httpClient = HttpClientFactory.CreateClient("VolvoProgrammingExerciseClientV1");
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);
            var res = await httpClient.DeleteAsync(
                requestUri: url
                , cancellationToken: cancellationTokenSource.Token);

            if (!res.IsSuccessStatusCode)
            {
                return 0;
            }

            var jsonStream = await res.Content.ReadAsStreamAsync();
            var deletedId = await JsonSerializer.DeserializeAsync<ulong>(
                utf8Json: jsonStream
                , options: JsonSerializerOptions
                , cancellationToken: cancellationTokenSource.Token);

            return deletedId;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            var clientException = new ClientExceptionModel
            {
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty
            };
            _ = await ClientExceptionService.AddAsync(clientException);
            return 0;
        }
    }

    public virtual async Task<T> GetAsync(ulong id)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("VolvoProgrammingExerciseClientV1");
            var url = $"{BaseUrl}?id={id}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);
            var res = await httpClient.SendAsync(req, cancellationTokenSource.Token);

            if (!res.IsSuccessStatusCode)
            {
                return Activator.CreateInstance<T>();
            }

            var jsonStream = await res.Content.ReadAsStreamAsync();

            if ((jsonStream?.Length ?? 0) < 1)
            {
                return Activator.CreateInstance<T>();
            }

            var model = (await JsonSerializer.DeserializeAsync<T>(
                utf8Json: jsonStream!
                , options: JsonSerializerOptions
                , cancellationToken: cancellationTokenSource.Token))
                ?? Activator.CreateInstance<T>();

            return model;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            var clientException = new ClientExceptionModel
            {
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty
            };
            _ = await ClientExceptionService.AddAsync(clientException);
            return Activator.CreateInstance<T>();
        }
    }

    public virtual async Task<BaseListModel<T>> ListAsync(
        uint pageNumber = BaseListModel<BaseModel>.DefaultPageNumber
        , ushort pageSize = BaseListModel<BaseModel>.DefaultPageSize)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("VolvoProgrammingExerciseClientV1");
            var url = $"{BaseUrl}/List?pageNumber={pageNumber}&pageSize={pageSize}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);
            var res = await httpClient.SendAsync(req, cancellationTokenSource.Token);

            if (!res.IsSuccessStatusCode)
            {
                return new BaseListModel<T>();
            }

            var jsonStream = await res.Content.ReadAsStreamAsync();

            if ((jsonStream?.Length ?? 0) < 1)
            {
                return new BaseListModel<T>();
            }

            var model = (await JsonSerializer.DeserializeAsync<BaseListModel<T>>(
                utf8Json: jsonStream!
                , options: JsonSerializerOptions
                , cancellationToken: cancellationTokenSource.Token))
                ?? new BaseListModel<T>();

            return model;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            var clientException = new ClientExceptionModel
            {
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty
            };
            _ = await ClientExceptionService.AddAsync(clientException);
            return new BaseListModel<T>();
        }
    }

    public virtual async Task<T> UpdateAsync(T model)
    {
        try
        {
            var httpClient = HttpClientFactory.CreateClient("VolvoProgrammingExerciseClientV1");
            using var cancellationTokenSource = new CancellationTokenSource(Timeout);
            var url = $"{BaseUrl}?id={model.Id}";
            var res = await httpClient.PutAsJsonAsync(
                requestUri: url
                , value: model
                , cancellationToken: cancellationTokenSource.Token);

            if (!res.IsSuccessStatusCode)
            {
                return Activator.CreateInstance<T>();
            }

            var jsonStream = await res.Content.ReadAsStreamAsync();
            model = (await JsonSerializer.DeserializeAsync<T>(
                utf8Json: jsonStream
                , options: JsonSerializerOptions
                , cancellationToken: cancellationTokenSource.Token))
                ?? Activator.CreateInstance<T>();

            return model!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            var clientException = new ClientExceptionModel
            {
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty
            };
            _ = await ClientExceptionService.AddAsync(clientException);
            return Activator.CreateInstance<T>();
        }
    }
    #endregion
}
