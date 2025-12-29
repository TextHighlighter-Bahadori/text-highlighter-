namespace ClojureHighlighter.Domain.ASTNode;

public abstract class AstNode
{
    public Token Token { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
}