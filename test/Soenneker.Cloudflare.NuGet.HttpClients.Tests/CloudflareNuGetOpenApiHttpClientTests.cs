using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Cloudflare.NuGet.HttpClients.Abstract;
using Soenneker.Cloudflare.NuGet.HttpClients.Registrars;

namespace Soenneker.Cloudflare.NuGet.HttpClients.Tests;

public sealed class CloudflareNuGetOpenApiHttpClientTests
{
    [Test]
    public async Task Configures_authenticated_cached_client()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Cloudflare:NuGet:BaseUrl"] = "https://nuget.test/",
            ["Cloudflare:NuGet:ApiKey"] = "test-key"
        }).Build());
        services.AddCloudflareNuGetOpenApiHttpClientAsSingleton();
        await using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ICloudflareNuGetOpenApiHttpClient>();
        var first = await factory.Get();
        await Assert.That(first.BaseAddress!.AbsoluteUri).IsEqualTo("https://nuget.test/");
        await Assert.That(first.DefaultRequestHeaders.Authorization!.ToString()).IsEqualTo("Bearer test-key");
        await Assert.That(ReferenceEquals(first, await factory.Get())).IsTrue();
    }
}
