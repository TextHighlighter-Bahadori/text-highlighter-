namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class ClassNode : AstNode
{
    public Token ClassKeyword { get; set; }
    public SymbolNode ClassName { get; set; }
    public List<Token> Modifiers { get; set; } = new();
    public List<SymbolNode> BaseTypes { get; set; } = new();
    public List<AstNode> Members { get; set; } = new();
}