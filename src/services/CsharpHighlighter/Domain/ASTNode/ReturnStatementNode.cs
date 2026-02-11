namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class ReturnStatementNode : AstNode
{
    public Token ReturnKeyword { get; set; }
    public AstNode Expression { get; set; }
}