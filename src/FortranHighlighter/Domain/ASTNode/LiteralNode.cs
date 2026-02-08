namespace FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

public class LiteralNode :AstNode
{
    public Token Token { get; set; }
    
    public LiteralNode(Token token)
    {
        Token = token;
    }
}