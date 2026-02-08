namespace ClojureHighlighter.Domain.ASTNode;

public class DefNode : AstNode
{
    public Token? DefKeyword { get; set; }
    public SymbolNode? VariableName { get; set; }
    public string? Docstring { get; set; }
    public AstNode? Value { get; set; }
}