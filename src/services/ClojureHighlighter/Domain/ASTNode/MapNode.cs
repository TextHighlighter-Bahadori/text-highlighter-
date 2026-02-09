namespace ClojureHighlighter.Domain.ASTNode;

public class MapNode:AstNode
{
    public List<AstNode> Elements { get; set; } = new List<AstNode>();
}