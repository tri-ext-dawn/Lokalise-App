using Lokalise.ReviewComments.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Lokalise.ReviewComments.Business;

public static class SetupDependencyInjection
{
    public static IServiceCollection AddReviewCommentsServices(this IServiceCollection services)
    {
        services.AddTransient<IApp, App>();
        return services;
    }
}