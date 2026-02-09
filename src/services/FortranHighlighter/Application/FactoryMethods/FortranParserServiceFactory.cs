using FortranHighlighter.Domain.Token;

namespace FortranHighlighter.Application.FactoryMethods;

public class FortranParserServiceFactory : IFortranParserServiceFactory
{
    public IFortranParserService Create(List<Token> tokens) => new FortranParserService(tokens);
}