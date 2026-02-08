namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class CallStatementNode: AstNode
{
    public Token CallKeyword { get; set; }
    public Token SubroutineName { get; set; }
    public List<AstNode> Arguments { get; set; } = new();
}