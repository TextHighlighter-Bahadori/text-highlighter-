namespace CsharpHighlighter.Application;
using CsharpHighlighter.Domain.Token;

public interface ICSharpTokenizerService
{
    public List<Token> Tokenize();
}