using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Serilog;

namespace Triton.Core
{
    public class Connection
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly CancellationToken _cancellationToken = default;

        public Connection(IConfiguration configuration, IHttpClientFactory factory)
        {
            _config = configuration;
            _httpClient = factory.CreateClient();

            //var startupPath = $"{System.IO.Directory.GetCurrentDirectory()}\\logs\\log.txt";

            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .WriteTo.File(startupPath, rollingInterval: RollingInterval.Day)
            //    .CreateLogger();
        }

        private static async Task<HttpResponseMessage> ErrorHelper<T>(HttpResponseMessage response, string apiUrl, T objectValue)
        {
            var content = await response.Content.ReadAsStringAsync();
            response.Content?.Dispose();
            Log.Error("Content: {@content}, apiUrl:  {@apiUrl}, Object Value:  {@objectValue}", content, apiUrl, objectValue);

            return response;
        }

        /// <summary>
        /// GetAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="path"></param>
        /// <param name="apiConfigKey"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string controller, string path, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}{path.Trim()}";
            var response = await _httpClient.GetAsync(apiUrl, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    throw new HttpRequestException($"{response.StatusCode}:{response.Content}");

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, cancellationToken: _cancellationToken);

            }

            var message = ErrorHelper(response, apiUrl, "");
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        /// <summary>
        /// For creating a new item over a web api using POST that will return the object passed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">Specify the name of the web api controller</param>
        /// <param name="path">Specify the path of the web api controller</param>
        /// <param name="postObject">Specify the object that you which to create</param>
        /// <param name="parameters">Specify additional parameters for your post</param>
        /// <param name="apiConfigKey">Specify the web api connection</param>
        /// <returns></returns>
        public async Task<T> PostAsync<T>(string controller, string path, T postObject, string parameters = null, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";
            if (parameters != null)
            {
                apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}{"?" + parameters.Trim()}";
            }
            var httpContent = new StringContent(JsonSerializer.Serialize(postObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, httpContent, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, cancellationToken: _cancellationToken);
            }

            var message = ErrorHelper(response, apiUrl, postObject);
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        /// <summary>
        /// For creating a new item over a web api using POST that will return a long
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">Specify the name of the web api controller</param>
        /// <param name="path">Specify the path of the web api controller</param>
        /// <param name="postObject">Specify the object that you which to create</param>
        /// <param name="apiConfigKey">Specify the web api connection</param>
        /// <returns></returns>
        public async Task<long> PostAsyncLong<T>(string controller, string path, T postObject, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";

            var httpContent = new StringContent(JsonSerializer.Serialize(postObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, httpContent, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<long>(responseStream, cancellationToken: _cancellationToken);
            }

            var message =await ErrorHelper(response, apiUrl, postObject);
            throw new HttpRequestException($"{response.StatusCode}:{message}");

        }

        /// <summary>
        /// For creating a new item over a web api using POST that will return a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">Specify the name of the web api controller</param>
        /// <param name="path">Specify the path of the web api controller</param>
        /// <param name="postObject">Specify the object that you which to create</param>
        /// <param name="apiConfigKey">Specify the web api connection</param>
        /// <returns></returns>
        public async Task<string> PostAsyncString<T>(string controller, string path, T postObject, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";

            var httpContent = new StringContent(JsonSerializer.Serialize(postObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, httpContent, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            var message = ErrorHelper(response, apiUrl, postObject);
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        public async Task<bool> PostAsyncBool<T>(string controller, string path, T postObject, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";

            var httpContent = new StringContent(JsonSerializer.Serialize(postObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, httpContent, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<bool>(responseStream, cancellationToken: _cancellationToken);
            }

            var message = ErrorHelper(response, apiUrl, postObject);
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        /// <summary>
        /// For updating an existing item over a web api using PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">Specify the name of the web api controller</param>
        /// <param name="path">Specify the path of the web api controller</param>
        /// <param name="putObject">Specify the object that you which to create</param>
        /// <param name="apiConfigKey"></param>
        /// <returns></returns>
        public async Task<bool> PutAsync<T>(string controller, string path, T putObject, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";

            var httpContent = new StringContent(JsonSerializer.Serialize(putObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(apiUrl, httpContent, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode) return true;

            var message = ErrorHelper(response, apiUrl, putObject);
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        public bool PutAsync2<T>(string controller, string path, T putObject, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";

            var httpContent = new StringContent(JsonSerializer.Serialize(putObject), Encoding.UTF8, "application/json");
            var response = _httpClient.PutAsync(apiUrl, httpContent);

            if (response.IsCompleted) return true;
            return false;
            //var message = ErrorHelper(response, apiUrl, putObject);
            //throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        /// <summary>
        /// PostAsJsonAsyncReturnDynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller">Specify the name of the webapi controller</param>
        /// <param name="path">Specify the path of the webapi controller</param>
        /// <param name="postObject">Specify the object that you which to create</param>
        /// <param name="apiConfigKey">Specify the web api</param>
        /// <returns></returns>
        public async Task<dynamic> PostAsyncDynamic<T,U>(string controller, string path, T postObject, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";
            var httpContent = new StringContent(JsonSerializer.Serialize(postObject), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, httpContent, _cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var result= await JsonSerializer.DeserializeAsync<U>(responseStream, cancellationToken: _cancellationToken);
                return result;
            }

            var message = ErrorHelper(response, apiUrl, postObject);
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }

        /// <summary>
        /// Delete a record and returns a bool indicating a success/failure
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="path"></param>
        /// <param name="apiConfigKey"></param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteAsync(string controller, string path, string apiConfigKey = "v1")
        {
            var apiUrl = $"{_config.GetSection("WebApiUrl").GetSection(apiConfigKey).Value}{controller.Trim()}/{path.Trim()}";
            var response = await _httpClient.DeleteAsync(apiUrl, _cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<bool>(responseStream, cancellationToken: _cancellationToken);
            }

            var message = ErrorHelper(response, apiUrl, "");
            throw new HttpRequestException($"{response.StatusCode}:{message}");
        }
    }
}