namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class IfStatementNode : AstNode
{
    public Token IfKeyword { get; set; }
    public AstNode Condition { get; set; }
    public List<AstNode> ThenBody { get; set; } = new();
    public List<AstNode> ElseBody { get; set; } = new();
}