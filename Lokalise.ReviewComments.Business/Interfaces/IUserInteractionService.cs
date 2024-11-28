namespace Lokalise.ReviewComments.Business.Interfaces;

public interface IUserInteractionService
{
    ConsoleKeyInfo GetCharacterFromUser();
    void PrintLine(string message);
    int ReadNumber();
}