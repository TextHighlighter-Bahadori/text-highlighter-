namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class IfStatementNode : AstNode
{
    public Token IfKeyword { get; set; }
    public AstNode Condition { get; set; }
    public List<AstNode> ThenBlock { get; set; } = new();
    public List<AstNode> ElseBlock { get; set; } = new();
}