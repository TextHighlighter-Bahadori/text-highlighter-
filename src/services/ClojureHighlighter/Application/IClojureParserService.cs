using ClojureHighlighter.Domain.ASTNode;

namespace ClojureHighlighter.Application;

public interface IClojureParserService
{
    public List<AstNode> Parse();
}