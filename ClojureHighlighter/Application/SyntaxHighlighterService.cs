using ClojureHighlighter.Application.FactoryMethods;
using ClojureHighlighter.Domain;
using ClojureHighlighter.Domain.ASTNode;

namespace ClojureHighlighter.Application;

public class HighlightedToken
{
    public Token Token { get; set; }
    public string Color { get; set; }
    public SymbolRole? Role { get; set; }
}

public class SyntaxHighlighterService : ISyntaxHighlighter
{
    private readonly IClojureParserServiceFactory _parserServiceFactory;
    private readonly IClojureTokenizerServiceFactory _tokenizerServiceFactory;
    private readonly Dictionary<int, string> _positionToColor;

    public SyntaxHighlighterService(IClojureParserServiceFactory parserServiceFactory,
        IClojureTokenizerServiceFactory tokenizerServiceFactory)
    {
        _positionToColor = new Dictionary<int, string>();
        _parserServiceFactory = parserServiceFactory;
        _tokenizerServiceFactory = tokenizerServiceFactory;
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

            
            /*
             * first we check whether a specific token has semantic meaning or not(it belongs to a node of AST), if not,
             * we simply assign an arbitrary color regardless of the structure(e.g, GetColorForTokenType() method)
             */
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
                Color = color,
                Role = null
            });
        }

        return result;
    }

    private void AssignColorsToNode(AstNode node)
    {
        switch (node)
        {
            case DefnNode defn:
                AssignColor(defn.DefnKeyword, "DarkMagenta", true);
                if (defn.FunctionName != null)
                    AssignColor(defn.FunctionName.Token, "DarkBlue", true);
                foreach (var param in defn.Parameters)
                    AssignColor(param.Token, "DarkCyan");
                foreach (var bodyExpr in defn.Body)
                    AssignColorsToNode(bodyExpr);
                break;

            case DefNode def:
                AssignColor(def.DefKeyword, "DarkMagenta", true);
                if (def.VariableName != null)
                    AssignColor(def.VariableName.Token, "DarkBlue", true);
                if (def.Value != null)
                    AssignColorsToNode(def.Value);
                break;

            case LetNode let:
                AssignColor(let.LetKeyword, "DarkMagenta", true);
                foreach (var binding in let.Bindings)
                {
                    if (binding.Symbol != null)
                        AssignColor(binding.Symbol.Token, "DarkCyan");
                    if (binding.Value != null)
                        AssignColorsToNode(binding.Value);
                }

                foreach (var bodyExpr in let.Body)
                    AssignColorsToNode(bodyExpr);
                break;

            case IfNode ifNode:
                AssignColor(ifNode.IfKeyword, "DarkMagenta", true);
                if (ifNode.Condition != null)
                    AssignColorsToNode(ifNode.Condition);
                if (ifNode.ThenBranch != null)
                    AssignColorsToNode(ifNode.ThenBranch);
                if (ifNode.ElseBranch != null)
                    AssignColorsToNode(ifNode.ElseBranch);
                break;

            case FunctionCallNode call:
                if (call.FunctionName != null)
                    AssignColor(call.FunctionName.Token, "Blue");
                foreach (var arg in call.Arguments)
                    AssignColorsToNode(arg);
                break;

            case LambdaNode lambda:
                AssignColor(lambda.FnKeyword, "DarkMagenta", true);
                foreach (var param in lambda.Parameters)
                    AssignColor(param.Token, "DarkCyan");
                foreach (var bodyExpr in lambda.Body)
                    AssignColorsToNode(bodyExpr);
                break;

            case NamespaceNode ns:
                AssignColor(ns.NsKeyword, "DarkMagenta", true);
                if (ns.NamespaceName != null)
                    AssignColor(ns.NamespaceName.Token, "DarkBlue", true);
                foreach (var decl in ns.Declarations)
                    AssignColorsToNode(decl);
                break;

            case SymbolNode symbol:
                string color = symbol.Role switch
                {
                    SymbolRole.FunctionName => "DarkBlue",
                    SymbolRole.FunctionCall => "Blue",
                    SymbolRole.Parameter => "DarkCyan",
                    SymbolRole.LocalBinding => "DarkCyan",
                    SymbolRole.Variable => "DarkBlue",
                    SymbolRole.Macro => "DarkMagenta",
                    _ => "Black"
                };
                AssignColor(symbol.Token, color, symbol.Role == SymbolRole.FunctionName);
                break;

            case LiteralNode literal:
                AssignColor(literal.Token, GetColorForTokenType(literal.LiteralType));
                break;

            case VectorNode vector:
                foreach (var elem in vector.Elements)
                    AssignColorsToNode(elem);
                break;

            case MapNode map:
                foreach (var elem in map.Elements)
                    AssignColorsToNode(elem);
                break;

            case ListNode list:
                foreach (var elem in list.Elements)
                    AssignColorsToNode(elem);
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
            TokenType.Number => "Cyan",
            TokenType.Keyword => "Blue",
            TokenType.Boolean => "Purple",
            TokenType.Nil => "Purple",
            TokenType.SpecialForm => "DarkMagenta",
            TokenType.Symbol => "Black",
            TokenType.Character => "DarkGreen",
            TokenType.Quote or TokenType.Deref or TokenType.Metadata or
                TokenType.Dispatch or TokenType.Unquote or TokenType.UnquoteSplicing or
                TokenType.SyntaxQuote => "DarkRed",
            _ => "Black"
        };
    }
}