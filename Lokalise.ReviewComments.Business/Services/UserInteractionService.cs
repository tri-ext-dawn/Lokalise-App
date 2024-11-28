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

    public long ReadNumber(long min, long max)
    {
        var input = Console.ReadLine();
        if(long.TryParse(input, out var number))
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

    public long ReadNumber(List<long> validValues)
    {
        var input = Console.ReadLine();
        if(long.TryParse(input, out var number))
        {
            if(validValues.Contains(number) is false)
            {
                PrintLine($"Please enter a valid number.");
                return ReadNumber(validValues);
            }
            return number;
        }
        
        PrintLine("Invalid input. Please enter a number.");
        return ReadNumber(validValues);
    }
}