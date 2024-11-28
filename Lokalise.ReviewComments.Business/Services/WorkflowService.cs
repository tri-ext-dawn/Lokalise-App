using System.Text.RegularExpressions;
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
        languages.Sort();
        _userInteractionService.PrintLine("Select language:");
        foreach (var language in languages)
        {
            _userInteractionService.PrintLine($"[{language.Id}] {language.ISO} {language.Name}");;
        }

        var selection = _userInteractionService.ReadNumber(languages.Select(x => x.Id).ToList());
        var selectedLanguage = languages.First(x => x.Id == selection);
        _logger.LogInformation("Selected language: {languages[selection].Id} {languages[selection].ISO} {languages[selection].Name}", selectedLanguage.Id, selectedLanguage.ISO, selectedLanguage.Name);
        return Task.FromResult(selectedLanguage);
    }

    public Task<bool> ProcessComment(Comment comment, Translation translation)
    {
        _userInteractionService.PrintLine($"Do you want to override: {translation.Id}? (y for yes, any other to skip)");
        PrintTranslation(translation.TranslationText);
        PrintComment(comment.Message);
        return Task.FromResult(true);
    }

    private void PrintComment(string message)
    {
        var regEx = new Regex("(<p>.*?<\\/p>)");
        var matches = regEx.Matches(message);
        var lines = matches.Select(x => x.Value.Substring(3, x.Value.Length - 7)).ToList();

        _userInteractionService.PrintLine("");
        _userInteractionService.PrintLine($"Comment:");
        _userInteractionService.PrintLine($"--------------------");
        foreach (var line in lines)
        {
            _userInteractionService.PrintLine(line);
        }
        _userInteractionService.PrintLine($"--------------------");
    }

    private void PrintTranslation(string message)
    {
        var lines = message.Split('\n');
        
        _userInteractionService.PrintLine("");
        _userInteractionService.PrintLine($"Translation:");
        _userInteractionService.PrintLine($"--------------------");
        foreach (var line in lines)
        {
            _userInteractionService.PrintLine(line);
        }
        _userInteractionService.PrintLine($"--------------------");
    }
}