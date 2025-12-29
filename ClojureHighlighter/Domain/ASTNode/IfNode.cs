namespace ClojureHighlighter.Domain.ASTNode;

public class IfNode
{
    public Token IfKeyword { get; set; }
    public AstNode Condition { get; set; }
    public AstNode ThenBranch { get; set; }
    public AstNode ElseBranch { get; set; }
}