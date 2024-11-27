using Lokalise.ReviewComments.Business.Interfaces;

namespace Lokalise.ReviewComments.Business;

public class App : IApp
{
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
    
    public Task Run()
    {
        throw new NotImplementedException();
    }
}