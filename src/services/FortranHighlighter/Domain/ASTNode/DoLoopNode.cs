namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class DoLoopNode : AstNode
{
    public Token DoKeyword { get; set; }
    public Token LoopVariable { get; set; }
    public AstNode StartValue { get; set; }
    public AstNode EndValue { get; set; }
    public AstNode StepValue { get; set; }
    public List<AstNode> Body { get; set; } = new();
}