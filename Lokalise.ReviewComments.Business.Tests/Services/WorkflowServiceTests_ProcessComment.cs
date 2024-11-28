using AutoFixture;
using Lokalise.ReviewComments.Business.Models;
using Moq;

namespace Lokalise.ReviewComments.Business.Tests.Services;

public class WorkflowServiceTests_ProcessComment : WorkflowServiceTests
{
    private string _translationText = @"TLine 1
T\n

TLine 4";
    private string _commentMessage = "<p>CLine 1</p><p>C\\n</p><p></p><p>CLine 4</p>";

    
    [Test]
    public async Task ProcessComment_PrintHeaderText()
    {
        // Arrange
        var translation = _fixture
            .Build<Translation>()
            .With(x => x.TranslationText, "Temp")
            .Create();

        var comment = _fixture
            .Build<Comment>()
            .With(x => x.Message, "<p>Temp</p>")
            .Create();

        // Act
        var result = await _sut.ProcessComment(comment, translation);
    
        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine($"Do you want to override: {translation.Id}? (y for yes, any other to skip)"), Times.Once);
        
    }
    
    [Test]
    public async Task ProcessComment_PrintTranslation()
    {
        // Arrange
        var translation = _fixture
            .Build<Translation>()
            .With(x => x.TranslationText, _translationText)
            .Create();

        var comment = _fixture
            .Build<Comment>()
            .With(x => x.Message, "<p>Temp</p>")
            .Create();

        // Act
        var result = await _sut.ProcessComment(comment, translation);
    
        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine($"Translation:"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"TLine 1"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"T\\n"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"TLine 4"), Times.Once);
    }
    
    [Test]
    public async Task ProcessComment_PrintComment()
    {
        // Arrange
        var translation = _fixture
            .Build<Translation>()
            .With(x => x.TranslationText, "Temp")
            .Create();

        var comment = _fixture
            .Build<Comment>()
            .With(x => x.Message, _commentMessage)
            .Create();

        // Act
        var result = await _sut.ProcessComment(comment, translation);
    
        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine($"Comment:"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"CLine 1"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"C\\n"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"CLine 4"), Times.Once);
    }
}