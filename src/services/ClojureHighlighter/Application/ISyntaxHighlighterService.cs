namespace ClojureHighlighter.Application;

public interface ISyntaxHighlighterService
{
    public List<HighlightedToken> HighlightCode(string code);
}