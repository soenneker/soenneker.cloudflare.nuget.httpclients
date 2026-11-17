using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.NuGet.HttpClients.Abstract;
using Soenneker.Utils.HttpClientCache.Registrar;

namespace Soenneker.Cloudflare.NuGet.HttpClients.Registrars;

/// <summary>
/// Registers the OpenAPI HttpClient wrapper for dependency injection.
/// </summary>
public static class CloudflareNuGetOpenApiHttpClientRegistrar
{
    /// <summary>
    /// Adds <see cref="CloudflareNuGetOpenApiHttpClient"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareNuGetOpenApiHttpClientAsSingleton(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton()
                .TryAddSingleton<ICloudflareNuGetOpenApiHttpClient, CloudflareNuGetOpenApiHttpClient>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="CloudflareNuGetOpenApiHttpClient"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareNuGetOpenApiHttpClientAsScoped(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton()
                .TryAddScoped<ICloudflareNuGetOpenApiHttpClient, CloudflareNuGetOpenApiHttpClient>();

        return services;
    }
}
