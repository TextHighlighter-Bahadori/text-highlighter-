namespace FortranHighlighter.Application;

public interface ISyntaxHighlighterService
{
    List<HighlightedToken> HighlightCode(string code);
}