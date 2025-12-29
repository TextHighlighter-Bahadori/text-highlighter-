namespace ClojureHighlighter.Domain.ASTNode;

public class NamespaceNode:AstNode
{
    public Token NsKeyword { get; set; }
    public SymbolNode NamespaceName { get; set; }
    public List<AstNode> Declarations { get; set; } = new List<AstNode>();
}