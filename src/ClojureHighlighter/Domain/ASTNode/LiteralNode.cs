namespace ClojureHighlighter.Domain.ASTNode;

public class LiteralNode:AstNode
{
    public object Value { get; set; }
    public TokenType LiteralType { get; set; }

    public LiteralNode(Token token)
    {
        Token = token;
        Value = token.Value;
        LiteralType = token.Type;
        StartPosition = token.Position;
        EndPosition = token.Position + token.Value.Length;
    }
}