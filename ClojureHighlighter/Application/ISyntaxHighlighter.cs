namespace ClojureHighlighter.Application;

public interface ISyntaxHighlighter
{
    public List<HighlightedToken> HighlightCode(string code);
}