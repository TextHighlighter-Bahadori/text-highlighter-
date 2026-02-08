using ClojureHighlighter.Domain;

namespace ClojureHighlighter.Application.FactoryMethods;

public interface IClojureParserServiceFactory
{
    public IClojureParserService Create(List<Token> tokens);
}