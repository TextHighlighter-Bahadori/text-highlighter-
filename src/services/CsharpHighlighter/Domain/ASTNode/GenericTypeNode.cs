namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class GenericTypeNode
{
    public SymbolNode TypeName { get; set; }
    public List<SymbolNode> TypeArguments { get; set; } = new();
}