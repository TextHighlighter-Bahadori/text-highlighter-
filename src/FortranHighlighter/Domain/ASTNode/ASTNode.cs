namespace FortranHighlighter.Domain.ASTNode;

public abstract class AstNode
{
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
}