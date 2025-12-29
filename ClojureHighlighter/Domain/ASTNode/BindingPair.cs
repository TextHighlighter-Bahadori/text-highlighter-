namespace ClojureHighlighter.Domain.ASTNode;

public class BindingPair: AstNode
{
    public SymbolNode Symbol { get; set; }
    public AstNode Value { get; set; }
}