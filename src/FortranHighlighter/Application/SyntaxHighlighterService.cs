using FortranHighlighter.Application.FactoryMethods;
using FortranHighlighter.Domain;
using FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

namespace FortranHighlighter.Application;

public class HighlightedToken
{
    public Token Token { get; set; }
    public string Color { get; set; }
}

public class SyntaxHighlighterService : ISyntaxHighlighterService
{
    private readonly Dictionary<int, string> _positionToColor;
    private readonly IFortranTokenizerServiceFactory _tokenizerServiceFactory;
    private readonly IFortranParserServiceFactory _parserServiceFactory;

    public SyntaxHighlighterService(IFortranTokenizerServiceFactory tokenizerServiceFactory,
        IFortranParserServiceFactory parserServiceFactory)
    {
        _tokenizerServiceFactory = tokenizerServiceFactory;
        _parserServiceFactory = parserServiceFactory;
        _positionToColor = new Dictionary<int, string>();
    }

    public List<HighlightedToken> HighlightCode(string code)
    {
        var tokenizerService = _tokenizerServiceFactory.Create(code);
        var tokens = tokenizerService.Tokenize();

        var parserService = _parserServiceFactory.Create(tokens);
        var ast = parserService.Parse();

        foreach (var node in ast)
        {
            AssignColorsToNode(node);
        }

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
            case ProgramNode program:
                AssignColor(program.ProgramKeyword, "DarkMagenta", true);
                AssignColor(program.ProgramName, "DarkBlue", true);
                foreach (var decl in program.Declarations)
                    AssignColorsToNode(decl);
                foreach (var stmt in program.Body)
                    AssignColorsToNode(stmt);
                break;

            case SubroutineNode subroutine:
                AssignColor(subroutine.SubroutineKeyword, "DarkMagenta", true);
                AssignColor(subroutine.SubroutineName, "DarkBlue", true);
                foreach (var param in subroutine.Parameters)
                    AssignColor(param, "DarkCyan");
                foreach (var decl in subroutine.Declarations)
                    AssignColorsToNode(decl);
                foreach (var stmt in subroutine.Body)
                    AssignColorsToNode(stmt);
                break;

            case FunctionNode function:
                AssignColor(function.FunctionKeyword, "DarkMagenta", true);
                AssignColor(function.FunctionName, "DarkBlue", true);
                foreach (var param in function.Parameters)
                    AssignColor(param, "DarkCyan");
                foreach (var decl in function.Declarations)
                    AssignColorsToNode(decl);
                foreach (var stmt in function.Body)
                    AssignColorsToNode(stmt);
                break;

            case VariableDeclarationNode varDecl:
                AssignColor(varDecl.TypeKeyword, "Blue", true);
                foreach (var variable in varDecl.Variables)
                    AssignColor(variable, "DarkCyan");
                break;

            case DoLoopNode doLoop:
                AssignColor(doLoop.DoKeyword, "DarkMagenta", true);
                AssignColor(doLoop.LoopVariable, "DarkCyan");
                AssignColorsToNode(doLoop.StartValue);
                AssignColorsToNode(doLoop.EndValue);
                AssignColorsToNode(doLoop.StepValue);
                foreach (var stmt in doLoop.Body)
                    AssignColorsToNode(stmt);
                break;

            case IfStatementNode ifStmt:
                AssignColor(ifStmt.IfKeyword, "DarkMagenta", true);
                AssignColorsToNode(ifStmt.Condition);
                foreach (var stmt in ifStmt.ThenBlock)
                    AssignColorsToNode(stmt);
                foreach (var stmt in ifStmt.ElseBlock)
                    AssignColorsToNode(stmt);
                break;

            case CallStatementNode call:
                AssignColor(call.CallKeyword, "DarkMagenta", true);
                AssignColor(call.SubroutineName, "Blue");
                foreach (var arg in call.Arguments)
                    AssignColorsToNode(arg);
                break;

            case AssignmentNode assignment:
                AssignColor(assignment.Variable, "DarkCyan");
                AssignColorsToNode(assignment.Expression);
                break;

            case FunctionCallNode funcCall:
                AssignColor(funcCall.FunctionName, "Blue");
                foreach (var arg in funcCall.Arguments)
                    AssignColorsToNode(arg);
                break;

            case BinaryExpressionNode binExpr:
                AssignColorsToNode(binExpr.Left);
                AssignColorsToNode(binExpr.Right);
                break;

            case LiteralNode literal:
                AssignColor(literal.Token, GetColorForTokenType(literal.Token.Type));
                break;

            case IdentifierNode identifier:
                string color = identifier.Role switch
                {
                    IdentifierRole.ProgramName => "DarkBlue",
                    IdentifierRole.SubroutineName => "DarkBlue",
                    IdentifierRole.FunctionName => "DarkBlue",
                    IdentifierRole.FunctionCall => "Blue",
                    IdentifierRole.VariableName => "DarkCyan",
                    IdentifierRole.Parameter => "DarkCyan",
                    IdentifierRole.LoopVariable => "DarkCyan",
                    _ => "Black"
                };
                AssignColor(identifier.Token, color);
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
            TokenType.Comment => "Gray",
            TokenType.String => "DarkGreen",
            TokenType.Integer or TokenType.Real => "Cyan",
            TokenType.Logical => "Purple",
            TokenType.Program or TokenType.Subroutine or TokenType.Function or
                TokenType.Do or TokenType.EndDo or TokenType.If or TokenType.Then or
                TokenType.Else or TokenType.ElseIf or TokenType.EndIf or TokenType.End or
                TokenType.Call or TokenType.Return or TokenType.Stop or TokenType.Continue => "DarkMagenta",
            TokenType.Integer_Keyword or TokenType.Real_Keyword or
                TokenType.Character_Keyword or TokenType.Logical_Keyword or
                TokenType.Dimension or TokenType.Parameter or TokenType.Implicit => "Blue",
            TokenType.Identifier => "Black",
            _ => "Black"
        };
    }
}