using System.Net;
using Lokalise.ReviewComments.Business.Extenstions;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Services;
using Lokalise.ReviewComments.Business.Services.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lokalise.ReviewComments.Business;

public static class SetupDependencyInjection
{
    public static IServiceCollection AddReviewCommentsServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<LokaliseSettings>(config.GetSection("LokaliseSettings"));

        var lokaliseSettings = config.GetSection("LokaliseSettings").Get<LokaliseSettings>();
        if(lokaliseSettings is null)
            throw new ArgumentNullException("LokaliseSettings");
        
        services.AddTransient<IApp, App>();
        services.AddTransient<ICommandService, CommandService>();
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<ILokaliseApiClient, LokaliseApiClient>();
        services.AddTransient<ILokaliseCookieClient, LokaliseCookieClient>();
        services.AddTransient<IUserInteractionService, UserInteractionService>();
        
        SetupHttp(services, lokaliseSettings);
        return services;
    }

    private static void SetupHttp(IServiceCollection services, LokaliseSettings lokaliseSettings)
    {
        services.AddHttpClient<ILokaliseApiClient, LokaliseApiClient>(client =>
        {
            client.DefaultRequestHeaders.Add("X-Api-Token", lokaliseSettings.ApiToken);
            // client.DefaultRequestHeaders.Add("accept", "application/json");
            // client.DefaultRequestHeaders.Add("content-type", "application/json");
            client.BaseAddress = new Uri(lokaliseSettings.BaseApiUrl);
        });
        // services.AddHttpClient<ILokaliseCookieClient, LokaliseCookieClient>(client =>
        // {
        //     client.DefaultRequestHeaders.Add("accept", "*/*");
        //     client.DefaultRequestHeaders.Add("host", lokaliseSettings.AppDomain);
        //     // client.DefaultRequestHeaders.Add("x-csrf-token", lokaliseSettings.XCsrfToken);
        //     client.BaseAddress = new Uri(lokaliseSettings.BaseCookieUrl);
        //
        //     var cookieString = "";
        //     foreach (var cookie in lokaliseSettings.Cookies)
        //     {
        //         if(cookieString.Length > 0)
        //             cookieString += "; ";
        //         cookieString += $"{cookie.Key}={cookie.Value}";
        //     }
        //     client.DefaultRequestHeaders.Add("cookie", cookieString.Trim());
        // });
    }
}