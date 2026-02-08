using ClojureHighlighter.Domain.ASTNode;

namespace ClojureHighlighter.Domain.ASTNode;

public class SymbolNode : AstNode
{
    public string Name { get; set; }
    public SymbolRole Role { get; set; }

    public SymbolNode(Token token, SymbolRole role = SymbolRole.Unknown)
    {
        Token = token;
        Name = token.Value;
        Role = role;
        StartPosition = token.Position;
        EndPosition = token.Position + token.Value.Length;
    }
}