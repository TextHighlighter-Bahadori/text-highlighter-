namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class FieldNode : AstNode
{
    public Token TypeToken { get; set; }
    public SymbolNode FieldName { get; set; }
    public List<Token> Modifiers { get; set; } = new();
    public AstNode Initializer { get; set; }
}