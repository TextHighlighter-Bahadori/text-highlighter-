namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class ForStatementNode : AstNode
{
    public Token ForKeyword { get; set; }
    public AstNode Initializer { get; set; }
    public AstNode Condition { get; set; }
    public AstNode Iterator { get; set; }
    public List<AstNode> Body { get; set; } = new();
}