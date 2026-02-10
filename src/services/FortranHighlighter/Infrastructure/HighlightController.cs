using System.Text;
using FortranHighlighter.Infrastructure.DTOs;
using FortranHighlighter.Application;
using Microsoft.AspNetCore.Mvc;

namespace FortranHighlighter.Infrastructure;

[ApiController]
[Route("/api/fortran/[controller]")]
public class HighlightController : ControllerBase
{
    private readonly ISyntaxHighlighterService _syntaxHighlighterService;
    private readonly ILogger<HighlightController> _logger;


    public HighlightController(ISyntaxHighlighterService syntaxHighlighterService, ILogger<HighlightController> logger)
    {
        _syntaxHighlighterService = syntaxHighlighterService;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult GetHighlightedCode(HighlightRequest highlightRequest)
    {
        if (highlightRequest.SourceCode.Length <= 10)
        {
            _logger.LogWarning("The source code length must be more than 10 characters");
            return Problem(
                statusCode: 400,
                title: "Invalid length",
                detail: "he source code length must be more than 10 characters");
        }
        _logger.LogInformation("request for highlighting the source code in string format");
        var highlightedTokens = _syntaxHighlighterService.HighlightCode(highlightRequest.SourceCode);
        return Ok(new
        {
            highlightedTokens = highlightedTokens
        });
    }


    [HttpPost]
    public async Task<IActionResult> GetHighlightedCode(IFormFile formFile)
    {
        if (formFile.Length <= 10)
        {
            _logger.LogWarning("The source code length must be more than 10 characters");
            return Problem(
                statusCode: 400,
                title: "Invalid length",
                detail: "he source code length must be more than 10 characters");
        }

        _logger.LogInformation("request for highlighting the source code in string format");
        byte[] content;
        using (var stream = formFile.OpenReadStream())
        {
            content = new byte[stream.Length];
            await stream.ReadExactlyAsync(content, 0, (int)stream.Length);
        }

        string sourceCode = Encoding.ASCII.GetString(content);
        var highlightedTokens = _syntaxHighlighterService.HighlightCode(sourceCode);
        return Ok(new
        {
            highlightedTokens = highlightedTokens
        });
    }
}