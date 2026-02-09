namespace ClojureHighlighter.Domain.ASTNode;

public class ListNode:AstNode
{
    public List<AstNode> Elements { get; set; } = new List<AstNode>();
    
}