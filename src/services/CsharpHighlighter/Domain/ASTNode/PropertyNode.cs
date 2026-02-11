namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class PropertyNode : AstNode
{
    public Token TypeToken { get; set; }
    public SymbolNode PropertyName { get; set; }
    public List<Token> Modifiers { get; set; } = new();
    public AstNode GetAccessor { get; set; }
    public AstNode SetAccessor { get; set; }
}