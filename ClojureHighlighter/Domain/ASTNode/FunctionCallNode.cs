namespace ClojureHighlighter.Domain.ASTNode;

public class FunctionCallNode : AstNode
{
    public SymbolNode FunctionName { get; set; }
    public List<AstNode> Arguments { get; set; } = new List<AstNode>();
}