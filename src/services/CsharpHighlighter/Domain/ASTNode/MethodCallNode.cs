namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class MethodCallNode : AstNode
{
    public SymbolNode MethodName { get; set; }
    public AstNode Target { get; set; }
    public List<AstNode> Arguments { get; set; } = new();
}