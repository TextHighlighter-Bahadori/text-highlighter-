namespace ClojureHighlighter.Domain.ASTNode;

public class VectorNode:AstNode
{
    public List<AstNode> Elements { get; set; } = new List<AstNode>();
}