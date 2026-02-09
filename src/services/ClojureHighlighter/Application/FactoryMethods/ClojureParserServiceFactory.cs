using ClojureHighlighter.Domain;

namespace ClojureHighlighter.Application.FactoryMethods;

public class ClojureParserServiceFactory : IClojureParserServiceFactory
{
    public IClojureParserService Create(List<Token> tokens) => new ClojureParserService(tokens);
}