namespace ClojureHighlighter.Domain.ASTNode;

public class IfNode : AstNode
{
    public Token IfKeyword { get; set; }
    public AstNode? Condition { get; set; }
    public AstNode? ThenBranch { get; set; }
    public AstNode? ElseBranch { get; set; }
}