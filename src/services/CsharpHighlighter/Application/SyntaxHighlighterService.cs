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
        switch (node)
        {
            case UsingNode usingNode:
                AssignColor(usingNode.UsingKeyword, "Purple", true);
                AssignColor(usingNode.NamespaceName.Token, "Black");
                break;

            case NamespaceNode ns:
                AssignColor(ns.NamespaceKeyword, "Purple", true);
                AssignColor(ns.NamespaceName.Token, "Black");
                foreach (var member in ns.Members)
                    AssignColorsToNode(member);
                break;

            case ClassNode classNode:
                foreach (var modifier in classNode.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(classNode.ClassKeyword, "Blue", true);
                AssignColor(classNode.ClassName.Token, "DarkCyan", true);
                foreach (var baseType in classNode.BaseTypes)
                    AssignColor(baseType.Token, "DarkCyan");
                foreach (var member in classNode.Members)
                    AssignColorsToNode(member);
                break;

            case InterfaceNode interfaceNode:
                foreach (var modifier in interfaceNode.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(interfaceNode.InterfaceKeyword, "Blue", true);
                AssignColor(interfaceNode.InterfaceName.Token, "DarkCyan", true);
                foreach (var baseInterface in interfaceNode.BaseInterfaces)
                    AssignColor(baseInterface.Token, "DarkCyan");
                foreach (var member in interfaceNode.Members)
                    AssignColorsToNode(member);
                break;

            case EnumNode enumNode:
                foreach (var modifier in enumNode.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(enumNode.EnumKeyword, "Blue", true);
                AssignColor(enumNode.EnumName.Token, "DarkCyan", true);
                foreach (var member in enumNode.Members)
                    AssignColor(member.Token, "DarkGray");
                break;

            case MethodNode method:
                foreach (var modifier in method.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(method.ReturnTypeToken, "Blue");
                AssignColor(method.MethodName.Token, "Yellow");
                foreach (var param in method.Parameters)
                    AssignColorsToNode(param);
                foreach (var stmt in method.Body)
                    AssignColorsToNode(stmt);
                break;

            case ParameterNode param:
                AssignColor(param.TypeToken, "Blue");
                AssignColor(param.ParameterName.Token, "DarkGray");
                break;

            case PropertyNode property:
                foreach (var modifier in property.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(property.TypeToken, "Blue");
                AssignColor(property.PropertyName.Token, "White");
                break;

            case FieldNode field:
                foreach (var modifier in field.Modifiers)
                    AssignColor(modifier, "Blue", true);
                AssignColor(field.TypeToken, "Blue");
                AssignColor(field.FieldName.Token, "White");
                AssignColorsToNode(field.Initializer);
                break;

            case VariableDeclarationNode varDecl:
                AssignColor(varDecl.TypeToken, "Blue");
                AssignColor(varDecl.VariableName.Token, "White");
                AssignColorsToNode(varDecl.Initializer);
                break;

            case MethodCallNode call:
                AssignColor(call.MethodName.Token, "Yellow");
                AssignColorsToNode(call.Target);
                foreach (var arg in call.Arguments)
                    AssignColorsToNode(arg);
                break;

            case IfStatementNode ifStmt:
                AssignColor(ifStmt.IfKeyword, "Purple", true);
                AssignColorsToNode(ifStmt.Condition);
                foreach (var stmt in ifStmt.ThenBody)
                    AssignColorsToNode(stmt);
                foreach (var stmt in ifStmt.ElseBody)
                    AssignColorsToNode(stmt);
                break;

            case ForStatementNode forStmt:
                AssignColor(forStmt.ForKeyword, "Purple", true);
                AssignColorsToNode(forStmt.Initializer);
                AssignColorsToNode(forStmt.Condition);
                AssignColorsToNode(forStmt.Iterator);
                foreach (var stmt in forStmt.Body)
                    AssignColorsToNode(stmt);
                break;

            case WhileStatementNode whileStmt:
                AssignColor(whileStmt.WhileKeyword, "Purple", true);
                AssignColorsToNode(whileStmt.Condition);
                foreach (var stmt in whileStmt.Body)
                    AssignColorsToNode(stmt);
                break;

            case ReturnStatementNode returnStmt:
                AssignColor(returnStmt.ReturnKeyword, "Purple", true);
                AssignColorsToNode(returnStmt.Expression);
                break;

            case BlockNode block:
                foreach (var stmt in block.Statements)
                    AssignColorsToNode(stmt);
                break;

            case AttributeNode attr:
                AssignColor(attr.AttributeName.Token, "Gray");
                foreach (var arg in attr.Arguments)
                    AssignColorsToNode(arg);
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
                AssignColor(genericType.TypeName.Token, "DarkCyan");
                foreach (var typeArg in genericType.TypeArguments)
                    AssignColor(typeArg.Token, "DarkCyan");
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