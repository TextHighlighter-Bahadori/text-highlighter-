using System.Text;
using ClojureHighlighter.Application;
using ClojureHighlighter.Infrastracture.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClojureHighlighter.Infrastracture;

[ApiController]
[Route("/api/clojure/[controller]")]
public class HighlightController : ControllerBase
{
    private readonly ISyntaxHighlighter _syntaxHighlighter;
    private readonly ILogger<HighlightController> _logger;


    public HighlightController(ISyntaxHighlighter syntaxHighlighter, ILogger<HighlightController> logger)
    {
        _syntaxHighlighter = syntaxHighlighter;
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
        var highlightedTokens = _syntaxHighlighter.HighlightCode(highlightRequest.SourceCode);
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
        var highlightedTokens = _syntaxHighlighter.HighlightCode(sourceCode);
        return Ok(new
        {
            highlightedTokens = highlightedTokens
        });
    }
}