namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class EnumNode : AstNode
{
    public Token EnumKeyword { get; set; }
    public SymbolNode EnumName { get; set; }
    public List<Token> Modifiers { get; set; } = new();
    public List<SymbolNode> Members { get; set; } = new();

}