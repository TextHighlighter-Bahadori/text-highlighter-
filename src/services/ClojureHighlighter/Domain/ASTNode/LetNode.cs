namespace ClojureHighlighter.Domain.ASTNode;

public class LetNode :AstNode
{
    public Token LetKeyword { get; set; }
    public List<BindingPair> Bindings { get; set; } = new List<BindingPair>();
    public List<AstNode> Body { get; set; } = new List<AstNode>();
}