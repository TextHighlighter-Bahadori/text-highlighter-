namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class SubroutineNode : AstNode
{
    public Token SubroutineKeyword { get; set; }
    public Token SubroutineName { get; set; }
    public List<Token> Parameters { get; set; } = new();
    public List<AstNode> Declarations { get; set; } = new();
    public List<AstNode> Body { get; set; } = new();
}