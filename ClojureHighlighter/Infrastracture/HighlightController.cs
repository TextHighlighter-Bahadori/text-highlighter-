using ClojureHighlighter.Application;
using Microsoft.AspNetCore.Mvc;

namespace ClojureHighlighter.Infrastracture;

[ApiController]
[Route("/api/clojure/[controller]")]
public class HighlightController
{
    private readonly ISyntaxHighlighter _syntaxHighlighter;
    private readonly ILogger<HighlightController> _logger;


    public HighlightController(ISyntaxHighlighter syntaxHighlighter, ILogger<HighlightController> logger)
    {
        _syntaxHighlighter = syntaxHighlighter;
        _logger = logger;
    }

    [HttpGet]
    public List<HighlightedToken> GetHighlightedCode([FromBody] string sourceCode)
    {
        var highlightedTokens = _syntaxHighlighter.HighlightCode(sourceCode);
        return highlightedTokens;
    }
}