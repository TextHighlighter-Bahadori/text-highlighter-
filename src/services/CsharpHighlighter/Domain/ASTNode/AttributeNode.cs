namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class AttributeNode : AstNode
{
    public SymbolNode AttributeName { get; set; }
    public List<AstNode> Arguments { get; set; } = new();
}