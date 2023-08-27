using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Net;

namespace BotProxy.BotServices;

public static class ConfigureServices
{
    private const string WebClientNames = "BotExternalClient";
    public static IServiceCollection AddBotExternalServices(this IServiceCollection services, IConfiguration configuration)
    {

        var section = configuration.GetSection("BotExternalService");
        var options = section.BindOptions<BotExternalServiceOptions>();

        services.Configure<BotExternalServiceOptions>(configuration.GetRequiredSection("BotExternalService"));

        services.AddHttpClient(WebClientNames, client =>
        {
            client.BaseAddress = new Uri(options.BaseUrl);
        })
        .AddPolicyHandler(Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => x.StatusCode is >= HttpStatusCode.NotFound or HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(options.RetryDelay), options.RetryCount)));

        services.AddScoped<IBotExternalService, BotExternalService>();

        return services;
    }

    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
       => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection section) where T : new()
    {
        var options = new T();
        section.Bind(options);
        return options;
    }
}
