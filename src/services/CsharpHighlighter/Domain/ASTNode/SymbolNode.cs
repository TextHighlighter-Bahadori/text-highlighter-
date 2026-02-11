namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class SymbolNode : AstNode
{
    public Token Token { get; set; }
    public SymbolRole Role { get; set; }

    public SymbolNode(Token token, SymbolRole role)
    {
        Token = token;
        Role = role;
        StartPosition = token.Position;
    }
}