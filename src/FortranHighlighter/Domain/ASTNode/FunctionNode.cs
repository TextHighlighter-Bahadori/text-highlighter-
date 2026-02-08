namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class FunctionNode : AstNode
{
    public Token FunctionKeyword { get; set; }
    public Token ReturnType { get; set; }
    public Token FunctionName { get; set; }
    public List<Token> Parameters { get; set; } = new();
    public List<AstNode> Declarations { get; set; } = new();
    public List<AstNode> Body { get; set; } = new();
}