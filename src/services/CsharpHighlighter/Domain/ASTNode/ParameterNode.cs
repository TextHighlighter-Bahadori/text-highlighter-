namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token; 
public class ParameterNode : AstNode
{
    public Token TypeToken { get; set; }
    public SymbolNode ParameterName { get; set; }
}