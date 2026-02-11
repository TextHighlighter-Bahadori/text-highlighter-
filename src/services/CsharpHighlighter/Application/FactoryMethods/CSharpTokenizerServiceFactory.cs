using CSharpHighlighter.Application;

namespace CsharpHighlighter.Application.FactoryMethods;

public class CSharpTokenizerServiceFactory : ICSharpTokenizerServiceFactory
{
    public ICSharpTokenizerService Create(string sourceCode) => new CSharpTokenizerService(sourceCode);
}