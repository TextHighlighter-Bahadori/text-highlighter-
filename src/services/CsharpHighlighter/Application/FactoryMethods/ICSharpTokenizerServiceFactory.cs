namespace CsharpHighlighter.Application.FactoryMethods;

public interface ICSharpTokenizerServiceFactory
{
    public ICSharpTokenizerService Create(string sourceCode);
}