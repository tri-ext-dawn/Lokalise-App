using Lokalise.ReviewComments.Business.Interfaces;

namespace Lokalise.ReviewComments.Business.Services;

public class UserInteractionService : IUserInteractionService
{
    public ConsoleKeyInfo GetCharacterFromUser()
    {
        var key = Console.ReadKey();
        Console.WriteLine();
        return key;
    }

    public void PrintLine(string message)
    {
        Console.WriteLine(message);
    }
}