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

    public App(IDataService dataService, IWorkflowService workflowService)
    {
        _dataService = dataService;
        _workflowService = workflowService;
    }
    
    public async Task Run()
    {
        var projectId = await _workflowService.SelectProject();

        var languages = await _dataService.GetLanguages(projectId);
        var language = await _workflowService.SelectLanguage(languages);
        
        var translations = await _dataService.GetTranslations(language.Id, projectId);
        
        var allComments = await _dataService.GetComments(projectId);
        var comments = allComments.Where(c => translations.Any(t => t.KeyId == c.KeyId)).ToList();

        foreach (var comment in comments)
        {
            await _workflowService.ProcessComment(comment);
        }
    }
}