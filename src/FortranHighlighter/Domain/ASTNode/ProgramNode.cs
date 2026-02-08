namespace FortranHighlighter.Domain.ASTNode;

public class ProgramNode
{
    public Token.Token ProgramKeyword { get; set; }
    public Token.Token ProgramName { get; set; }
    public List<AstNode> Declarations { get; set; } = new();
    public List<AstNode> Body { get; set; } = new();
}