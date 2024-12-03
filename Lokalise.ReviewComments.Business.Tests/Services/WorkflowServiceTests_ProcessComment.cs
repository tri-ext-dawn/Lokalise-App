using AutoFixture;
using FluentAssertions;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lokalise.ReviewComments.Business.Tests.Services;

public class WorkflowServiceTests_ProcessComment : WorkflowServiceTests
{
    private string _translationText = @"TLine 1
T\n

TLine 4";
    private string _commentMessage = "<p>CLine 1</p><p>C\\n</p><p>&gt;</p><p>CLine 4</p>";
    private string _expectedMessage = @"CLine 1
C\n
>
CLine 4";
    private string _projectId = "1234.1234";
    
    [Test]
    public async Task ProcessComment_PrintHeaderText()
    {
        // Arrange
        var translation = CreateTranslation();
        var comment = CreateComment();
        UserSaidNo();

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine($"Do you want to override: {comment.AttachPointName}? (y for yes, any other to skip)"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"Author: {comment.Author.Name}"), Times.Once);
    }
    
    [Test]
    public async Task ProcessComment_PrintTranslation()
    {
        // Arrange
        var translation = CreateTranslation(_translationText);
        var comment = CreateComment(_commentMessage);
        UserSaidNo();

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
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
        var translation = CreateTranslation();
        var comment = CreateComment(_commentMessage);
        UserSaidNo();

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine($"Comment:"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"CLine 1"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"C\\n"), Times.Once);
        _mockUserInteractionService.Verify(x => x.PrintLine($"CLine 4"), Times.Once);
    }
    
    [Test]
    public async Task ProcessComment_Skip_DoesNothing()
    {
        // Arrange
        var translation = CreateTranslation();
        var comment = CreateComment(_commentMessage);
        UserSaidNo();

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        result.Should().BeTrue();
        VerifyUpdate(Times.Never());
        VerifyResolved(Times.Never());
    }

    [Test]
    public async Task ProcessComment_Override_CorrectCommands()
    {
        // Arrange
        var translation = CreateTranslation();
        var comment = CreateComment(_commentMessage);
        UserSaidYes();
        SetUpdateStatus(true);
        SetResolvedStatus(true);

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        _mockCommandService.Verify(x => x.UpdateTranslation(translation.Id, _expectedMessage, _projectId), Times.Once());
        _mockCommandService.Verify(x => x.ResolveComment(comment.Id, translation.Id, _projectId), Times.Once());
    }

    [Test]
    public async Task ProcessComment_Override_UpdatesAndResolves()
    {
        // Arrange
        var translation = CreateTranslation();
        var comment = CreateComment(_commentMessage);
        UserSaidYes();
        SetUpdateStatus(true);
        SetResolvedStatus(true);

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        result.Should().BeTrue();
        VerifyUpdate(Times.Once());
        VerifyResolved(Times.Once());
    }
    
    [Test]
    public async Task ProcessComment_UpdateFails_PrintMessageAndDoNotCallResolve()
    {
        // Arrange
        var translation = CreateTranslation();
        var comment = CreateComment(_commentMessage);
        UserSaidYes();
        SetUpdateStatus(false);
        SetResolvedStatus(true);

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        result.Should().BeFalse();
        VerifyUpdate(Times.Once());
        VerifyResolved(Times.Never());
        _mockUserInteractionService.Verify(x => x.PrintLine($"Failed to update translation {translation.Id}"), Times.Once);
        VerifyLog(LogLevel.Error, $"Failed to update translation {translation.Id} with message: {_expectedMessage}");
    }
    
    [Test]
    public async Task ProcessComment_UpdateFails_PrintMessage()
    {
        // Arrange
        var translation = CreateTranslation();
        var comment = CreateComment(_commentMessage);
        UserSaidYes();
        SetUpdateStatus(true);
        SetResolvedStatus(false);

        // Act
        var result = await _sut.ProcessComment(comment, translation, _projectId);
    
        // Assert
        result.Should().BeFalse();
        VerifyUpdate(Times.Once());
        VerifyResolved(Times.Once());
        _mockUserInteractionService.Verify(x => x.PrintLine($"Updated translation {translation.Id} but failed to resolve comment {comment.Id}"), Times.Once);
        VerifyLog(LogLevel.Critical, $"Updated translation {translation.Id} but failed to resolve comment {comment.Id}");
    }

    private void SetResolvedStatus(bool status)
    {
        _mockCommandService.Setup(x => 
                x.ResolveComment(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(status);
    }

    private void SetUpdateStatus(bool status)
    {
        _mockCommandService.Setup(x => 
                x.UpdateTranslation(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(status);
    }

    private Translation CreateTranslation(string translationText = "Temp")
    {
        return _fixture
            .Build<Translation>()
            .With(x => x.TranslationText, translationText)
            .Create();
    }

    private Comment CreateComment(string commentMessage = "<p>Temp</p>")
    {
        return _fixture
            .Build<Comment>()
            .With(x => x.Message, commentMessage)
            .Create();
    }
    
    private void UserSaidNo()
    {
        _mockUserInteractionService.Setup(x => x.GetCharacterFromUser()).Returns(new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false));
    }
    
    private void UserSaidYes()
    {
        _mockUserInteractionService.Setup(x => x.GetCharacterFromUser()).Returns(new ConsoleKeyInfo('y', ConsoleKey.Y, false, false, false));
    }

    private void VerifyUpdate(Times times)
    {
        _mockCommandService.Verify(x => 
            x.UpdateTranslation(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), times);
    }

    private void VerifyResolved(Times times)
    {
        _mockCommandService.Verify(x => 
            x.ResolveComment(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>()), times);    
    }

}