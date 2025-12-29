namespace ClojureHighlighter.Domain.ASTNode;

public class DefnNode: AstNode
{
    public Token DefnKeyword { get; set; }
    public SymbolNode FunctionName { get; set; }
    public string Docstring { get; set; }
    public List<SymbolNode> Parameters { get; set; } = new List<SymbolNode>();
    public List<AstNode> Body { get; set; } = new List<AstNode>();
    public bool IsPrivate { get; set; }
}