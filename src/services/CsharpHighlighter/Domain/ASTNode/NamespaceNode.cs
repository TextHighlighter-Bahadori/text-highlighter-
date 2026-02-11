namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class NamespaceNode : AstNode
{
    public Token NamespaceKeyword { get; set; }
    public SymbolNode NamespaceName { get; set; }
    public List<AstNode> Members { get; set; } = new();
}