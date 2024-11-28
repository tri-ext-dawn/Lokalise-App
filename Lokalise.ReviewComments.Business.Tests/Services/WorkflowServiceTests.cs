using AutoFixture;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Lokalise.ReviewComments.Business.Tests.Services;

public class WorkflowServiceTests
{
    protected Fixture _fixture;
    protected Mock<ILogger<WorkflowService>> _mockLogger;
    protected Mock<ICommandService> _mockCommandService;
    protected Mock<IUserInteractionService> _mockUserInteractionService;
    protected WorkflowSettings _options;
    
    protected IWorkflowService _sut;
    
    protected void VerifyLog(LogLevel logLevel, string message, int times = 1) => Utilities.VerifyLog(_mockLogger, logLevel, message, times);
    protected T DeepClone<T>(T source) => Utilities.DeepClone(source);

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _mockLogger = new Mock<ILogger<WorkflowService>>();
        _mockCommandService = new Mock<ICommandService>();
        _mockUserInteractionService = new Mock<IUserInteractionService>();
        _options = new WorkflowSettings
        {
            Projects = new Dictionary<string, string>
            {
                {"Project1", "1"},
                {"Project2", "2"}
            }
        };
        var mockOptions = new Mock<IOptions<WorkflowSettings>>();
        mockOptions.Setup(x => x.Value).Returns(_options);
        _sut = new WorkflowService(_mockLogger.Object, mockOptions.Object, _mockCommandService.Object, _mockUserInteractionService.Object);
    }
}