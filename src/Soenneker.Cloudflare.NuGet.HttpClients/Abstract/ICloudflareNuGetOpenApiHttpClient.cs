using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Cloudflare.NuGet.HttpClients.Abstract;

/// <summary>
/// A thread-safe, authenticated HTTP client for the private Cloudflare NuGet feed.
/// </summary>
public interface ICloudflareNuGetOpenApiHttpClient: IDisposable, IAsyncDisposable
{
    /// <summary>Gets the cached client configured with Cloudflare:NuGet:BaseUrl and Cloudflare:NuGet:ApiKey. Redirects are disabled.</summary>
    ValueTask<HttpClient> Get(CancellationToken cancellationToken = default);
}
