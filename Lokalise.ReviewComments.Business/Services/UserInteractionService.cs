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

    public int ReadNumber()
    {
        var input = Console.ReadLine();
        if(int.TryParse(input, out var number))
        {
            return number;
        }
        
        PrintLine("Invalid input. Please enter a number.");
        return ReadNumber();
    }
}