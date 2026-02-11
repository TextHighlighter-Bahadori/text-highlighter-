namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token; 

public class UsingNode : AstNode
{
    public Token UsingKeyword { get; set; }
    public SymbolNode NamespaceName { get; set; }
}