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
    private readonly ICommandService _commandService;

    public App(IDataService dataService, ICommandService commandService)
    {
        _dataService = dataService;
        _commandService = commandService;
    }
    
    public async Task Run()
    {
        var projectId = "3174716666ba0500034d17.77744948";
        var languageId = 665;
        var translationId = 4850905278L;
        
        var translations = await _dataService.GetTranslations(languageId, projectId);
        var languages = await _dataService.GetLanguages(projectId);
        var comments = await _dataService.GetComments(projectId);

        var comment = comments.First();
        var translation = translations.First(x => x.Id == translationId);
        var updated = await _commandService.UpdateTranslation(translation.Id, comment.Message, projectId);
        var resolved = await _commandService.ResolveComment(comment.Id, translation.Id, projectId);
        
        throw new NotImplementedException();
    }
}