namespace FortranHighlighter.Application;

using FortranHighlighter.Domain.Token;

public interface IFortranTokenizerService
{
    List<Token> Tokenize();
}