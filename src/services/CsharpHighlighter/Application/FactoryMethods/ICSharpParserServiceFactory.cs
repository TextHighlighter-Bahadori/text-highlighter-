namespace CsharpHighlighter.Application.FactoryMethods;
using CsharpHighlighter.Domain.Token;
public interface ICSharpParserServiceFactory
{
    public ICSharpParserService Create(List<Token> tokens);
}