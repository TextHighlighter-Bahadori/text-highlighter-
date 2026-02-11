namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class WhileStatementNode : AstNode
{
    public Token WhileKeyword { get; set; }
    public AstNode Condition { get; set; }
    public List<AstNode> Body { get; set; } = new();
}