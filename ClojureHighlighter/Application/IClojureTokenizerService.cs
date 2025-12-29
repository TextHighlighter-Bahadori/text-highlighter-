using ClojureHighlighter.Domain;

namespace ClojureHighlighter.Application;

public interface IClojureTokenizerService
{
    public List<Token> Tokenize();
}