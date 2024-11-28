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
        
        var translations = await _dataService.GetTranslations(665, "3174716666ba0500034d17.77744948");
        var comments = await _dataService.GetComments("3174716666ba0500034d17.77744948");
        var success = await _commandService.ResolveComment("f19d48cc-ab6c-4377-a308-0c07bc71a035", 4850905278L,
            "3174716666ba0500034d17.77744948");
        
        throw new NotImplementedException();
    }
}