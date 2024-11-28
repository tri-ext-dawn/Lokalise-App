using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lokalise.ReviewComments.Business.Tests.Services;

public class WorkflowServiceTests_SelectProject : WorkflowServiceTests
{
    [Test]
    public async Task SelectProject_PrintsProjects()
    {
        // Arrange
        var projects = _fixture.Create<Dictionary<string, string>>();
        _options.Projects = projects;

        // Act
        var result = await _sut.SelectProject();

        // Assert
        _mockUserInteractionService.Verify(x => x.PrintLine("Select project:"), Times.Once);
        for (int i = 0; i < projects.Count; i++)
        {
            var project = projects.ElementAt(i);
            _mockUserInteractionService.Verify(x => x.PrintLine($"[{i}] {project.Key}"), Times.Once);
        }
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task SelectProject_ValidSelection_ReturnsSelection(int selection)
    {
        // Arrange
        var projects = _fixture.Create<Dictionary<string, string>>();
        _options.Projects = projects;
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<long>(), It.IsAny<long>())).Returns(selection);

        // Act
        var result = await _sut.SelectProject();

        // Assert
        result.Should().Be(projects.ElementAt(selection).Value);
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public async Task SelectProject_ValidSelection_LogSelection(int selection)
    {
        // Arrange
        var projects = _fixture.Create<Dictionary<string, string>>();
        _options.Projects = projects;
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<long>(), It.IsAny<long>())).Returns(selection);

        // Act
        var result = await _sut.SelectProject();

        // Assert
        VerifyLog(LogLevel.Information, $"Selected project: {projects.ElementAt(selection).Value}");
    }
    
    [Test]
    public async Task SelectProject_ValidSelection_CallsReadNumberCorrectly()
    {
        // Arrange
        var projects = _fixture.Create<Dictionary<string, string>>();
        _options.Projects = projects;
        _mockUserInteractionService.Setup(x => x.ReadNumber(It.IsAny<long>(), It.IsAny<long>())).Returns(0);

        // Act
        var result = await _sut.SelectProject();

        // Assert
        _mockUserInteractionService.Verify(x => x.ReadNumber(0, projects.Count), Times.Once);
    }
}