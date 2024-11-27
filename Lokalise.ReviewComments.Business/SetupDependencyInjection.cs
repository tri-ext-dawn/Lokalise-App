using System.Net;
using Lokalise.ReviewComments.Business.Extenstions;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Services.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lokalise.ReviewComments.Business;

public static class SetupDependencyInjection
{
    public static IServiceCollection AddReviewCommentsServices(this IServiceCollection services, IConfiguration config)
    {
        var lokaliseSettings = config.GetSection("LokaliseSettings").Get<LokaliseSettings>();
        services.Configure<LokaliseSettings>(config.GetSection("LokaliseSettings"));
        if(lokaliseSettings is null)
            throw new ArgumentNullException("LokaliseSettings");
        
        services.AddTransient<IApp, App>();
        services.AddHttpClient<ILokaliseApiClient, LokaliseApiClient>(client =>
        {
            client.DefaultRequestHeaders.Add("X-Api-Token", lokaliseSettings.ApiToken);
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("content-type", "application/json");
            client.BaseAddress = new Uri(lokaliseSettings.BaseApiUrl);
        });
        services.AddHttpClient<ILokaliseCookieClient, LokaliseCookieClient>(client =>
        {
            client.DefaultRequestHeaders.Add("accept", "/*");
            client.DefaultRequestHeaders.Add("x-csrf-token", lokaliseSettings.XCsrfToken);
            client.BaseAddress = new Uri(lokaliseSettings.BaseCookieUrl);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer().Apply(container =>
            {
                foreach (var cookie in lokaliseSettings.Cookies)
                {
                    container.Add(new Uri(lokaliseSettings.AppDomain), 
                        new Cookie(cookie.Key, cookie.Value) { Domain = lokaliseSettings.AppDomain });
                }
            })
        });
        return services;
    }
}