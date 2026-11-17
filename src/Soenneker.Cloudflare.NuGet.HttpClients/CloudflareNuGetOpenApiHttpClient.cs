using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Extensions.Configuration;
using Soenneker.Cloudflare.NuGet.HttpClients.Abstract;
using Soenneker.Utils.HttpClientCache.Abstract;

namespace Soenneker.Cloudflare.NuGet.HttpClients;

public sealed class CloudflareNuGetOpenApiHttpClient : ICloudflareNuGetOpenApiHttpClient
{
    private readonly IHttpClientCache _httpClientCache;
    private readonly IConfiguration _config;
    private readonly string _cacheKey = $"{nameof(CloudflareNuGetOpenApiHttpClient)}:{Guid.NewGuid():N}";


    public CloudflareNuGetOpenApiHttpClient(IHttpClientCache httpClientCache, IConfiguration config)
    {
        _httpClientCache = httpClientCache;
        _config = config;
    }

    public ValueTask<HttpClient> Get(CancellationToken cancellationToken = default)
    {
        return _httpClientCache.Get(_cacheKey, (config: _config, baseUrl: _config.GetStringStrict("Cloudflare:NuGet:BaseUrl")), static state =>
        {
            var apiKey = state.config.GetStringStrict("Cloudflare:NuGet:ApiKey");
            string authHeaderName = state.config["Cloudflare:NuGet:AuthHeaderName"] ?? "Authorization";
            string authHeaderValueTemplate = state.config["Cloudflare:NuGet:AuthHeaderValueTemplate"] ?? "Bearer {token}";
            string authHeaderValue = authHeaderValueTemplate.Replace("{token}", apiKey, StringComparison.Ordinal);

            return new HttpClientOptions
            {
                BaseAddress = ValidateBaseUrl(state.baseUrl),
                AllowAutoRedirect = false,
                DefaultRequestHeaders = new Dictionary<string, string>
                {
                    {authHeaderName, authHeaderValue},
                }
            };
        }, cancellationToken);
    }

    private static Uri ValidateBaseUrl(string value)
    {
        var uri = new Uri(value, UriKind.Absolute);
        if ((uri.Scheme != Uri.UriSchemeHttps && !(uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback)) ||
            !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment) || uri.AbsolutePath != "/")
            throw new ArgumentException("Cloudflare:NuGet:BaseUrl must be an HTTPS origin (HTTP is allowed for loopback).", nameof(value));
        return uri;
    }

    public void Dispose()
    {
        _httpClientCache.RemoveSync(_cacheKey);
    }

    public ValueTask DisposeAsync()
    {
        return _httpClientCache.Remove(_cacheKey);
    }
}
