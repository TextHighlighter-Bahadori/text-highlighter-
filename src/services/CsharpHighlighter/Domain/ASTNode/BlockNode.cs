namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class BlockNode : AstNode
{
    public List<AstNode> Statements { get; set; } = new();
}