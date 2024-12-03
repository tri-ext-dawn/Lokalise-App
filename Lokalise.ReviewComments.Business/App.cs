using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;

namespace Lokalise.ReviewComments.Business;

// App Flow
//  - Get all necessary Data
//  - For each comment
//      - Display: Current Translation, Comment, and Key
//      - Ask for Override
//      - If Override
//          - Update Translation
//          - Resolve Comment
//      - If not Override
//          - Log that it was skipped
//          - Skip
//  - Done

public class App : IApp
{
    
    private readonly IDataService _dataService;
    private readonly IWorkflowService _workflowService;
    private readonly IUserInteractionService _userInteractionService;

    public App(IDataService dataService, IWorkflowService workflowService, IUserInteractionService userInteractionService)
    {
        _dataService = dataService;
        _workflowService = workflowService;
        _userInteractionService = userInteractionService;
    }
    
    public async Task Run()
    {
        var projectId = await _workflowService.SelectProject();
        _userInteractionService.PrintLine("");

        var allComments = await _dataService.GetComments(projectId);
        var languages = await _dataService.GetLanguages(projectId);

        PrintAllComments(allComments, languages);

        var language = await _workflowService.SelectLanguage(languages);
        var translations = await _dataService.GetTranslations(language.Id, projectId);
        var comments = allComments.Where(c => c.LangId == language.Id && translations.Any(t => t.KeyId == c.KeyId)).ToList();

        for (int i = 0; i < comments.Count; i++)
        {
            _userInteractionService.PrintLine("");
            _userInteractionService.PrintLine("");
            var comment = comments[i];
            var translation = translations.First(t => t.KeyId == comment.KeyId);

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            _userInteractionService.PrintLine($"=== {i+1}/{comments.Count} ===");
            Console.ForegroundColor = oldColor;
            
            await _workflowService.ProcessComment(comment, translation, projectId);
        }
    }

    private void PrintAllComments(List<Comment> comments, List<Language> languages)
    {
        var byLang = comments.GroupBy(x => x.LangId);
        foreach (var lang in byLang)
        {
            var language = languages.FirstOrDefault(x => x.Id == lang.Key);
            if(language is not null)
                _userInteractionService.PrintLine($"Language: {language.ISO} ({lang.Key}) has {lang.Count()} comments");
        }

        var noLangComments = byLang.FirstOrDefault(x => x.Key.HasValue is false).ToList();
        var ivarComment = 0;
        var otherComment = 0;
        foreach (var comment in noLangComments)
        {
            if (comment.Author.UserId == 545675)
                ivarComment++;
            else 
                otherComment++;
        }
        
        _userInteractionService.PrintLine($"<Empty> Language has {ivarComment} comments from Ivar");
        _userInteractionService.PrintLine($"<Empty> Language has {otherComment} comments from others");
    }
}