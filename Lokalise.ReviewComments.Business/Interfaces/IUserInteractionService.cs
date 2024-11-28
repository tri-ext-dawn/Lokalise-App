namespace Lokalise.ReviewComments.Business.Interfaces;

public interface IUserInteractionService
{
    ConsoleKeyInfo GetCharacterFromUser();
    void PrintLine(string message);
    long ReadNumber(long min, long max);
    long ReadNumber(List<long> validValues);
}