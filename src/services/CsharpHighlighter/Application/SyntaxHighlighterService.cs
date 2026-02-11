using CsharpHighlighter.Application;
using CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

namespace CSharpHighlighter.Application;

public class HighlightedToken
{
    public Token Token { get; set; }
    public string Color { get; set; }
}

public class SyntaxHighlighterService : ISyntaxHighlighterService
{
    private readonly Dictionary<int, string> _positionToColor;

    public SyntaxHighlighterService()
    {
        _positionToColor = new Dictionary<int, string>();
    }

    public List<HighlightedToken> HighlightCode(string code)
    {
        var tokenizerService = new CSharpTokenizerService(code);
        var tokens = tokenizerService.Tokenize();

        var parserService = new CSharpParserService(tokens);
        var ast = parserService.Parse();

        // Assign colors based on AST nodes
        foreach (var node in ast)
        {
            AssignColorsToNode(node);
        }

        // Build result
        var result = new List<HighlightedToken>();
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.EOF)
                continue;

            string color;
            if (_positionToColor.ContainsKey(token.Position))
            {
                color = _positionToColor[token.Position];
            }
            else
            {
                color = GetColorForTokenType(token.Type);
            }

            result.Add(new HighlightedToken
            {
                Token = token,
                Color = color
            });
        }

        return result;
    }

    private void AssignColorsToNode(AstNode node)
    {
        if (node == null)
            return;
            
        switch (node)
        {
            case UsingNode usingNode:
                AssignColor(usingNode.UsingKeyword, "Purple", true);
                if (usingNode.NamespaceName != null)
                    AssignColor(usingNode.NamespaceName.Token, "Black");
                break;

            case NamespaceNode ns:
                AssignColor(ns.NamespaceKeyword, "Purple", true);
                if (ns.NamespaceName != null)
                    AssignColor(ns.NamespaceName.Token, "Black");
                foreach (var member in ns.Members)
                {
                    if (member != null)
                        AssignColorsToNode(member);
                }
                break;

            case ClassNode classNode:
                foreach (var modifier in classNode.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(classNode.ClassKeyword, "Blue", true);
                if (classNode.ClassName != null)
                    AssignColor(classNode.ClassName.Token, "DarkCyan", true);
                foreach (var baseType in classNode.BaseTypes)
                {
                    if (baseType != null)
                        AssignColor(baseType.Token, "DarkCyan");
                }
                foreach (var member in classNode.Members)
                {
                    if (member != null)
                        AssignColorsToNode(member);
                }
                break;

            case InterfaceNode interfaceNode:
                foreach (var modifier in interfaceNode.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(interfaceNode.InterfaceKeyword, "Blue", true);
                if (interfaceNode.InterfaceName != null)
                    AssignColor(interfaceNode.InterfaceName.Token, "DarkCyan", true);
                foreach (var baseInterface in interfaceNode.BaseInterfaces)
                {
                    if (baseInterface != null)
                        AssignColor(baseInterface.Token, "DarkCyan");
                }
                foreach (var member in interfaceNode.Members)
                {
                    if (member != null)
                        AssignColorsToNode(member);
                }
                break;

            case EnumNode enumNode:
                foreach (var modifier in enumNode.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(enumNode.EnumKeyword, "Blue", true);
                if (enumNode.EnumName != null)
                    AssignColor(enumNode.EnumName.Token, "DarkCyan", true);
                foreach (var member in enumNode.Members)
                {
                    if (member != null)
                        AssignColor(member.Token, "DarkGray");
                }
                break;

            case MethodNode method:
                foreach (var modifier in method.Modifiers)
                    AssignColor(modifier, "Blue", true);
                if (method.ReturnTypeToken != null)
                    AssignColor(method.ReturnTypeToken, "Blue");
                if (method.MethodName != null)
                    AssignColor(method.MethodName.Token, "Yellow");
                foreach (var param in method.Parameters)
                {
                    if (param != null)
                        AssignColorsToNode(param);
                }
                foreach (var stmt in method.Body)
                {
                    if (stmt != null)
                        AssignColorsToNode(stmt);
                }
                break;

            case ParameterNode param:
                if (param.TypeToken != null)
                    AssignColor(param.TypeToken, "Blue");
                if (param.ParameterName != null)
                    AssignColor(param.ParameterName.Token, "DarkGray");
                break;

            case PropertyNode property:
                foreach (var modifier in property.Modifiers)
                    AssignColor(modifier, "Blue", true);
                if (property.TypeToken != null)
                    AssignColor(property.TypeToken, "Blue");
                if (property.PropertyName != null)
                    AssignColor(property.PropertyName.Token, "White");
                break;

            case FieldNode field:
                foreach (var modifier in field.Modifiers)
                    AssignColor(modifier, "Blue", true);
                if (field.TypeToken != null)
                    AssignColor(field.TypeToken, "Blue");
                if (field.FieldName != null)
                    AssignColor(field.FieldName.Token, "White");
                if (field.Initializer != null)
                    AssignColorsToNode(field.Initializer);
                break;

            case VariableDeclarationNode varDecl:
                if (varDecl.TypeToken != null)
                    AssignColor(varDecl.TypeToken, "Blue");
                if (varDecl.VariableName != null)
                    AssignColor(varDecl.VariableName.Token, "White");
                if (varDecl.Initializer != null)
                    AssignColorsToNode(varDecl.Initializer);
                break;

            case MethodCallNode call:
                if (call.MethodName != null)
                    AssignColor(call.MethodName.Token, "Yellow");
                if (call.Target != null)
                    AssignColorsToNode(call.Target);
                foreach (var arg in call.Arguments)
                {
                    if (arg != null)
                        AssignColorsToNode(arg);
                }
                break;

            case IfStatementNode ifStmt:
                AssignColor(ifStmt.IfKeyword, "Purple", true);
                if (ifStmt.Condition != null)
                    AssignColorsToNode(ifStmt.Condition);
                foreach (var stmt in ifStmt.ThenBody)
                {
                    if (stmt != null)
                        AssignColorsToNode(stmt);
                }
                foreach (var stmt in ifStmt.ElseBody)
                {
                    if (stmt != null)
                        AssignColorsToNode(stmt);
                }
                break;

            case ForStatementNode forStmt:
                AssignColor(forStmt.ForKeyword, "Purple", true);
                if (forStmt.Initializer != null)
                    AssignColorsToNode(forStmt.Initializer);
                if (forStmt.Condition != null)
                    AssignColorsToNode(forStmt.Condition);
                if (forStmt.Iterator != null)
                    AssignColorsToNode(forStmt.Iterator);
                foreach (var stmt in forStmt.Body)
                {
                    if (stmt != null)
                        AssignColorsToNode(stmt);
                }
                break;

            case WhileStatementNode whileStmt:
                AssignColor(whileStmt.WhileKeyword, "Purple", true);
                if (whileStmt.Condition != null)
                    AssignColorsToNode(whileStmt.Condition);
                foreach (var stmt in whileStmt.Body)
                {
                    if (stmt != null)
                        AssignColorsToNode(stmt);
                }
                break;

            case ReturnStatementNode returnStmt:
                AssignColor(returnStmt.ReturnKeyword, "Purple", true);
                if (returnStmt.Expression != null)
                    AssignColorsToNode(returnStmt.Expression);
                break;

            case BlockNode block:
                foreach (var stmt in block.Statements)
                {
                    if (stmt != null)
                        AssignColorsToNode(stmt);
                }
                break;

            case AttributeNode attr:
                if (attr.AttributeName != null)
                    AssignColor(attr.AttributeName.Token, "Gray");
                foreach (var arg in attr.Arguments)
                {
                    if (arg != null)
                        AssignColorsToNode(arg);
                }
                break;

            case SymbolNode symbol:
                string color = symbol.Role switch
                {
                    SymbolRole.ClassName => "DarkCyan",
                    SymbolRole.InterfaceName => "DarkCyan",
                    SymbolRole.MethodName => "Yellow",
                    SymbolRole.MethodCall => "Yellow",
                    SymbolRole.PropertyName => "White",
                    SymbolRole.FieldName => "White",
                    SymbolRole.VariableName => "White",
                    SymbolRole.ParameterName => "DarkGray",
                    SymbolRole.LocalVariable => "White",
                    SymbolRole.NamespaceName => "Black",
                    SymbolRole.TypeName => "DarkCyan",
                    SymbolRole.EnumName => "DarkCyan",
                    SymbolRole.AttributeName => "Gray",
                    _ => "White"
                };
                AssignColor(symbol.Token, color, 
                    symbol.Role == SymbolRole.ClassName || 
                    symbol.Role == SymbolRole.InterfaceName ||
                    symbol.Role == SymbolRole.EnumName);
                break;

            case LiteralNode literal:
                AssignColor(literal.Token, GetColorForTokenType(literal.LiteralType));
                break;

            case GenericTypeNode genericType:
                if (genericType.TypeName != null)
                    AssignColor(genericType.TypeName.Token, "DarkCyan");
                foreach (var typeArg in genericType.TypeArguments)
                {
                    if (typeArg != null)
                        AssignColor(typeArg.Token, "DarkCyan");
                }
                break;
        }
    }

    private void AssignColor(Token token, string color, bool bold = false)
    {
        _positionToColor[token.Position] = bold ? $"Bold{color}" : color;
    }

    private static string GetColorForTokenType(TokenType type)
    {
        return type switch
        {
            TokenType.Keyword => "Blue",
            TokenType.String => "DarkRed",
            TokenType.Number => "Green",
            TokenType.Character => "DarkRed",
            TokenType.Boolean => "Blue",
            TokenType.Null => "Blue",
            TokenType.Comment => "Green",
            TokenType.Preprocessor => "Gray",
            TokenType.Attribute => "Gray",
            TokenType.Operator => "Gray",
            TokenType.Identifier => "White",
            _ => "Black"
        };
    }
}