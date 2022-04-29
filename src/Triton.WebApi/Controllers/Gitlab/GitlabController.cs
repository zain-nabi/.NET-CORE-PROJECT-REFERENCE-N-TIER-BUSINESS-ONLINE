using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Triton.Model.Utils;

namespace Triton.WebApi.Controllers.Gitlab
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitlabController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly CancellationToken _cancellationToken = default;

        private readonly Uri _url = new Uri("http://texzadcelastic/api/v4/");
        private const string Page = "100";

        public GitlabController(IConfiguration configuration, IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get Gitlab issues", Description = "Get Gitlab issues")]
        public async Task<ActionResult<dynamic>> Get(string filter)
        {
            var apiUrl = $"http://texzadcelastic/api/v4/issues?scope=all&private_token=rj__psNbUafpFXcebSXD&per_page=100{filter}";
            var response = await _httpClient.GetAsync(apiUrl, _cancellationToken).ConfigureAwait(false);

            var model = new List<GitlabModel.Root>();

            if (response.IsSuccessStatusCode)
            {
                var pages = int.Parse(response.Headers.GetValues("X-Total-Pages").FirstOrDefault() ?? string.Empty);

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                model.AddRange(await JsonSerializer.DeserializeAsync<List<GitlabModel.Root>>(responseStream, cancellationToken: _cancellationToken));

                if (pages <= 0) return model;
                for (var i = 2; i <= pages; i++)
                {
                    apiUrl = $"http://texzadcelastic/api/v4/issues?scope=all&private_token=rj__psNbUafpFXcebSXD&per_page=100&page={i}{filter}";
                    response = await _httpClient.GetAsync(apiUrl, _cancellationToken).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode) continue;
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        throw new HttpRequestException($"{response.StatusCode}:{response.Content}");

                    await using var stream = await response.Content.ReadAsStreamAsync();
                    //return await JsonSerializer.DeserializeAsync<dynamic>(responseStream, cancellationToken: _cancellationToken);
                    model.AddRange(await JsonSerializer.DeserializeAsync<List<GitlabModel.Root>>(stream, cancellationToken: _cancellationToken));
                }
            }

            return model;
        }

        [HttpGet("GetProjectByFilter")]
        [SwaggerOperation(Summary = "Get Gitlab issues by project_id and additional filters", Description = "Get Gitlab issues by project_id and additional filters")]
        public async Task<ActionResult<dynamic>> GetProjectByFilter(int projectId, string filter)
        {
            var url = new Uri(_url, $"projects/{projectId}/issues?scope=all&private_token=rj__psNbUafpFXcebSXD{filter}");
            var apiUrl = $"{url}&per_page={Page}";

            var response = await _httpClient.GetAsync(apiUrl, _cancellationToken).ConfigureAwait(false);

            var model = new List<GitlabModel.Root>();

            if (!response.IsSuccessStatusCode) return model;

            var pages = int.Parse(response.Headers.GetValues("X-Total-Pages").FirstOrDefault() ?? string.Empty);

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            model.AddRange(await JsonSerializer.DeserializeAsync<List<GitlabModel.Root>>(responseStream, cancellationToken: _cancellationToken));

            if (pages <= 0) return model;
            for (var i = 2; i <= pages; i++)
            {
                apiUrl = $"{url}&per_page={i}";
                response = await _httpClient.GetAsync(apiUrl, _cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode) continue;
                if (response.StatusCode == HttpStatusCode.NoContent)
                    throw new HttpRequestException($"{response.StatusCode}:{response.Content}");

                await using var stream = await response.Content.ReadAsStreamAsync();
                model.AddRange(await JsonSerializer.DeserializeAsync<List<GitlabModel.Root>>(stream, cancellationToken: _cancellationToken));
            }

            return model;
        }

        [HttpGet("Project")]
        [SwaggerOperation(Summary = "Get Gitlab projects", Description = "Get Gitlab projects")]
        public async Task<ActionResult<List<GitlabProject.Root>>> GetProject()
        {
            var apiUrl = "http://texzadcelastic/api/v4/projects?scope=all&private_token=rj__psNbUafpFXcebSXD&per_page=50000";
            var response = await _httpClient.GetAsync(apiUrl, _cancellationToken).ConfigureAwait(false);

            var model = new List<GitlabProject.Root>();

            if (!response.IsSuccessStatusCode) return model;
            await using var stream = await response.Content.ReadAsStreamAsync();
            model.AddRange(await JsonSerializer.DeserializeAsync<List<GitlabProject.Root>>(stream, cancellationToken: _cancellationToken));
            return model;
        }
    }
}
