namespace FortranHighlighter.Application.FactoryMethods;
using FortranHighlighter.Domain.Token;
public interface IFortranParserServiceFactory
{
    public IFortranParserService Create(List<Token> tokens);
}