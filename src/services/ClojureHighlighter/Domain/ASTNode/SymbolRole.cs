namespace ClojureHighlighter.Domain.ASTNode;

public enum SymbolRole
{
    Unknown,
    FunctionName,
    FunctionCall,
    Parameter,
    LocalBinding,
    NamespaceAlias,
    Macro,
    SpecialForm,
    Variable
}