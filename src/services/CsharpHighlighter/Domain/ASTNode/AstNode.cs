namespace CsharpHighlighter.Domain.ASTNode;

public enum SymbolRole
{
    Unknown,
    ClassName,
    InterfaceName,
    MethodName,
    MethodCall,
    PropertyName,
    FieldName,
    VariableName,
    ParameterName,
    LocalVariable,
    NamespaceName,
    TypeName,
    EnumName,
    AttributeName
}

public abstract class AstNode
{
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
}