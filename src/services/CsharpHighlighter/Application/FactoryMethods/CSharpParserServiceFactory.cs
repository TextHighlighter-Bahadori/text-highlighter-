using CSharpHighlighter.Application;

namespace CsharpHighlighter.Application.FactoryMethods;
using CsharpHighlighter.Domain.Token;

public class CSharpParserServiceFactory : ICSharpParserServiceFactory
{
    public ICSharpParserService Create(List<Token> tokens) =>  new CSharpParserService(tokens);
}