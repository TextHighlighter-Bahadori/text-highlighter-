using CSharpHighlighter.Application;

namespace CsharpHighlighter.Application;

public interface ISyntaxHighlighterService
{
    public List<HighlightedToken> HighlightCode(string code);
}