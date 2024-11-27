namespace Lokalise.ReviewComments.Business.Extenstions;

public static class CookieContainerExtensions
{
    public static T Apply<T>(this T item, Action<T> action)
    {
        action(item);
        return item;
    }
}