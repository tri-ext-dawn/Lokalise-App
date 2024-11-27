using Lokalise.ReviewComments.App.Models;
using Lokalise.ReviewComments.App.Services;

namespace Lokalise.ReviewComments.App;

public class Old
{
    public async Task OldMethod()
    {
        var lokaliseClient = new LokaliseClient();
        var translations = await lokaliseClient.GetTranslations();
        var languages = Language.Languages;
        var comments = await lokaliseClient.GetComments();

        await Process();

        async Task Process()
        {
            foreach (var comment in comments)
            {
                var language = languages.First(x => x.Id == comment.LangId);
                var translation = translations.First(x => x.KeyId == comment.KeyId && string.Equals(x.LanguageIso, language.ISO));

                var strippedMessage = StripMessage(comment.Message);
                Console.WriteLine(comment.AttachPointName);
                Console.WriteLine($"{language.ISO}: {translation.TranslationText}");
                Console.WriteLine($"{language.ISO}: {strippedMessage}");
                Console.WriteLine($"{translation.Id}");
                Console.WriteLine("O for override and any other to skip");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.Key == ConsoleKey.O)
                {
                    var success = await lokaliseClient.UpdateTranslation(translation.Id, strippedMessage);
                    if (success)
                        Console.WriteLine("Translation updated");
                    else 
                        Console.WriteLine("!!! Translation could not be updated");
            
                    success = await lokaliseClient.ResolveComment(comment.Id, translation.Id);
                    if (success)
                        Console.WriteLine("Comment resolved");
                    else
                        Console.WriteLine("!!! Comment could not be resolved");

                }
            }
        }

        string StripMessage(string message)
        {
            return message.Substring(3, message.Length - 7);
        }
    }
}