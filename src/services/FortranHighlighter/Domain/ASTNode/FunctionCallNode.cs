namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class FunctionCallNode : AstNode
{
    public Token FunctionName { get; set; }
    public List<AstNode> Arguments { get; set; } = new();
}