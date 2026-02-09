namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token; 

public class VariableDeclarationNode : AstNode
{
    public Token TypeKeyword { get; set; }
    public List<Token> Variables { get; set; } = new();
    public List<Token> Dimensions { get; set; } = new();
    public bool IsParameter { get; set; }
}