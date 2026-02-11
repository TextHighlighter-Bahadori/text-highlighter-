using CsharpHighlighter.Domain.ASTNode;

namespace CsharpHighlighter.Application;

public interface ICSharpParserService
{
    public List<AstNode> Parse();
}