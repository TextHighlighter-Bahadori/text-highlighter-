namespace ClojureHighlighter.Application.FactoryMethods;

public interface IClojureTokenizerServiceFactory
{
    public IClojureTokenizerService Create(string sourceCode);
}