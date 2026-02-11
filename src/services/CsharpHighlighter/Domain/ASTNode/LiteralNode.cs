namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class LiteralNode : AstNode
{
    public Token Token { get; set; }
    public TokenType LiteralType { get; set; }

    public LiteralNode(Token token)
    {
        Token = token;
        LiteralType = token.Type;
        StartPosition = token.Position;
    }
}