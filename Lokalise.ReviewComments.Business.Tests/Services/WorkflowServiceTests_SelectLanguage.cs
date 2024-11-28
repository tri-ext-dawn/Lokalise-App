using AutoFixture;
using FluentAssertions;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lokalise.ReviewComments.Business.Tests.Services;

public class WorkflowServiceTests_SelectLanguage : WorkflowServiceTests
{
    [Test]
    public async Task SelectLanguage_PrintsProjects()
    {
        // Arrange
        var languages = _fixture.CreateMany<Language>().ToList();
        var selectedLanguage = languages.First();
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<List<long>>())).Returns(selectedLanguage.Id);

        // Act
        var result = await _sut.SelectLanguage(languages);

        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine("Select language:"), Times.Once);
        foreach (var language in languages)
            _mockUserInteractionService.Verify(x => x.PrintLine($"[{language.Id}] {language.ISO} {language.Name}"), Times.Once);
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task SelectLanguage_ValidSelection_ReturnsSelection(int selection)
    {
        // Arrange
        var languages = _fixture.CreateMany<Language>().ToList();
        var selectedLanguage = languages[selection];
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<List<long>>())).Returns(selectedLanguage.Id);

        // Act
        var result = await _sut.SelectLanguage(languages);

        // Assert
        result.Should().Be(selectedLanguage);
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task SelectLanguage_ValidSelection_LogSelection(int selection)
    {
        // Arrange
        var languages = _fixture.CreateMany<Language>().ToList();
        var selectedLanguage = languages[selection];
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<List<long>>())).Returns(selectedLanguage.Id);

        // Act
        var result = await _sut.SelectLanguage(languages);

        // Assert
        VerifyLog(LogLevel.Information, $"Selected language: {selectedLanguage.Id} {selectedLanguage.ISO} {selectedLanguage.Name}");
    }
    
    [Test]
    public async Task SelectLanguage_ValidSelection_CallsReadNumberCorrectly()
    {
        // Arrange
        var languages = _fixture.CreateMany<Language>().ToList();
        var selectedLanguage = languages.First();
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<List<long>>())).Returns(selectedLanguage.Id);

        // Act
        var result = await _sut.SelectLanguage(languages);

        // Assert
        _mockUserInteractionService.Verify(x => x.ReadNumber(It.Is<List<long>>(v => v.SequenceEqual(languages.Select(l => l.Id)))), Times.Once);
    }
    
}