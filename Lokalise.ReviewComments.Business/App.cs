using Lokalise.ReviewComments.Business.Interfaces;

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
        var languages = await _dataService.GetLanguages(projectId);
        var language = await _workflowService.SelectLanguage(languages);
        
        var translations = await _dataService.GetTranslations(language.Id, projectId);
        
        var allComments = await _dataService.GetComments(projectId);
        var comments = allComments.Where(c => c.LangId == language.Id && translations.Any(t => t.KeyId == c.KeyId)).ToList();

        for (int i = 0; i < comments.Count; i++)
        {
            _userInteractionService.PrintLine("");
            _userInteractionService.PrintLine("");
            var comment = comments[i];
            var translation = translations.First(t => t.KeyId == comment.KeyId);
            
            _userInteractionService.PrintLine($"=== {i+1}/{comments.Count} ===");
            await _workflowService.ProcessComment(comment, translation, projectId);
        }
    }
}