namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class MethodNode : AstNode
{
    public Token ReturnTypeToken { get; set; }
    public SymbolNode MethodName { get; set; }
    public List<Token> Modifiers { get; set; } = new();
    public List<ParameterNode> Parameters { get; set; } = new();
    public List<AstNode> Body { get; set; } = new();
}