namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class AssignmentNode : AstNode
{
    public Token Variable { get; set; }
    public Token AssignmentOp { get; set; }
    public AstNode Expression { get; set; }
}