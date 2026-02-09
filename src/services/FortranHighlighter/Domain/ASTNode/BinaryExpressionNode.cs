namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class BinaryExpressionNode : AstNode
{
    public AstNode Left { get; set; }
    public Token Operator { get; set; }
    public AstNode Right { get; set; }
}