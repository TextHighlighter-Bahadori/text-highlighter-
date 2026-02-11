namespace CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

public class VariableDeclarationNode : AstNode
{
    public Token TypeToken { get; set; }
    public SymbolNode VariableName { get; set; }
    public AstNode Initializer { get; set; }
}