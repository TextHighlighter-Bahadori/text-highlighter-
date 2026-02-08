namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class IdentifierNode : AstNode
{
    public Token Token { get; set; }
    public IdentifierRole Role { get; set; }
    
    public IdentifierNode(Token token, IdentifierRole role = IdentifierRole.Unknown)
    {
        Token = token;
        Role = role;
    }
}

public enum IdentifierRole
{
    Unknown,
    ProgramName,
    SubroutineName,
    FunctionName,
    FunctionCall,
    VariableName,
    Parameter,
    LoopVariable
}