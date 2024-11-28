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

    public int ReadNumber(int min, int max)
    {
        var input = Console.ReadLine();
        if(int.TryParse(input, out var number))
        {
            if(number < min || number >= max)
            {
                PrintLine($"Please enter a number between {min} and {max-1}.");
                return ReadNumber(min, max);
            }
            return number;
        }
        
        PrintLine("Invalid input. Please enter a number.");
        return ReadNumber(min, max);
    }
}