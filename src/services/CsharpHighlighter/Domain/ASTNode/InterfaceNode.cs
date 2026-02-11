using CsharpHighlighter.Domain.ASTNode;

namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class InterfaceNode : AstNode
{
    public Token InterfaceKeyword { get; set; }
    public SymbolNode InterfaceName { get; set; }
    public List<Token> Modifiers { get; set; } = new();
    public List<SymbolNode> BaseInterfaces { get; set; } = new();
    public List<AstNode> Members { get; set; } = new();
}