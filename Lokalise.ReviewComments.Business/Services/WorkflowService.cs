using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lokalise.ReviewComments.Business.Services;

public class WorkflowService : IWorkflowService
{
    private readonly ILogger<WorkflowService> _logger;
    private readonly ICommandService _commandService;
    private readonly IUserInteractionService _userInteractionService;
    private readonly WorkflowSettings _options;

    public WorkflowService(ILogger<WorkflowService> logger, IOptions<WorkflowSettings> options, ICommandService commandService, IUserInteractionService userInteractionService)
    {
        _logger = logger;
        _commandService = commandService;
        _userInteractionService = userInteractionService;
        _options = options.Value;
    }
    
    public Task<string> SelectProject()
    {
        _userInteractionService.PrintLine("Select project:");
        var projects = _options.Projects.ToArray();
        for (int i = 0; i < projects.Length; i++)
        {
            var project = projects.ToArray()[i];
            _userInteractionService.PrintLine($"[{i}] {project.Key}");;
        }

        var selection = _userInteractionService.ReadNumber(0, projects.Length);
        var selectedProject = projects[selection];
        _logger.LogInformation("Selected project: {ProjectId}", selectedProject.Value);
        return Task.FromResult(selectedProject.Value);
    }

    public Task<Language> SelectLanguage(List<Language> languages)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ProcessComment(Comment comment)
    {
        throw new NotImplementedException();
    }
}