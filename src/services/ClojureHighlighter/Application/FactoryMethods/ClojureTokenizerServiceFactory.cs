namespace ClojureHighlighter.Application.FactoryMethods;

public class ClojureTokenizerServiceFactory : IClojureTokenizerServiceFactory
{
    public IClojureTokenizerService Create(string sourceCode) => new ClojureTokenizerService(sourceCode);
}