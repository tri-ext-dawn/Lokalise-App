using System.Net;
using System.Text.RegularExpressions;
using Lokalise.ReviewComments.Business.Interfaces;
using Lokalise.ReviewComments.Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.CompilerServices;

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
        _logger.LogInformation("Selected language: {LanguageId} {LanguageISO} {LanguageName}", selectedLanguage.Id, selectedLanguage.ISO, selectedLanguage.Name);
        return Task.FromResult(selectedLanguage);
    }

    public async Task<bool> ProcessComment(Comment comment, Translation translation, string projectId)
    {
        var commentLines = GetCommentLines(comment.Message);

        _userInteractionService.PrintLine($"Do you want to override: {translation.Id}? (y for yes, any other to skip)");
        PrintTranslation(translation.TranslationText);
        PrintComment(commentLines);

        var input = _userInteractionService.GetCharacterFromUser();
        if (input.KeyChar == 'y')
        {
            var message = CreateMessage(commentLines);
            
            var success = await _commandService.UpdateTranslation(translation.Id, message, projectId);
            if (success is false)
            {
                _userInteractionService.PrintLine($"Failed to update translation {translation.Id}");
                _logger.LogError("Failed to update translation {TranslationId} with message: {UpdateTranslation}", translation.Id, message);
                return false;
            }
            
            success = await _commandService.ResolveComment(comment.Id, translation.Id, projectId);
            if (success is false)
            {
                _userInteractionService.PrintLine($"Updated translation {translation.Id} but failed to resolve comment {comment.Id}");
                _logger.LogCritical("Updated translation {TranslationId} but failed to resolve comment {CommentId}", translation.Id, comment.Id);
                return false;
            }
        }
        
        return true;
    }

    private string CreateMessage(List<string> commentLines)
    {
        var res = string.Join('\n', commentLines);
        return res;
    }

    private List<string> GetCommentLines(string message)
    {
        var decodedMessage = DecodeHtmlEntities(message);
        var regEx = new Regex("(<p>.*?<\\/p>)");
        var matches = regEx.Matches(decodedMessage);
        var lines = matches.Select(x => x.Value.Substring(3, x.Value.Length - 7)).ToList();
        return lines;
    }
    
    public string DecodeHtmlEntities(string input)
    {
        return WebUtility.HtmlDecode(input);
    }

    private void PrintComment(List<string> lines)
    {
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