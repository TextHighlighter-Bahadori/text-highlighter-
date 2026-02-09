namespace ClojureHighlighter.Domain.ASTNode;

public class LambdaNode:AstNode
{
    public Token FnKeyword { get; set; }
    public List<SymbolNode> Parameters { get; set; } = new List<SymbolNode>();
    public List<AstNode> Body { get; set; } = new List<AstNode>();

}