using CsharpHighlighter.Application;
using CsharpHighlighter.Domain.ASTNode;
using CsharpHighlighter.Domain.Token;

namespace CSharpHighlighter.Application;

public class CSharpParserService : ICSharpParserService
{
    private List<Token> _tokens;
    private int _current;

    private readonly HashSet<string> _modifiers = new()
    {
        "public", "private", "protected", "internal", "static", "virtual",
        "override", "abstract", "sealed", "readonly", "const", "extern",
        "unsafe", "volatile", "async", "partial"
    };

    private readonly HashSet<string> _typeKeywords = new()
    {
        "class", "interface", "struct", "enum", "delegate", "record"
    };

    public CSharpParserService(List<Token> tokens)
    {
        _tokens = tokens;
        _current = 0;
    }

    public List<AstNode> Parse()
    {
        var nodes = new List<AstNode>();

        while (!IsAtEnd())
        {
            SkipWhitespaceAndComments();
            if (!IsAtEnd())
            {
                var node = ParseTopLevel();
                if (node != null)
                    nodes.Add(node);
            }
        }

        return nodes;
    }

    private AstNode? ParseTopLevel()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
            return null;

        Token current = CurrentToken();

        if (current.Type == TokenType.Keyword && current.Value == "using")
            return ParseUsing();

        if (current.Type == TokenType.Keyword && current.Value == "namespace")
            return ParseNamespace();

        if (current.Type == TokenType.Attribute)
            return ParseAttribute();

        if (IsTypeDeclaration())
            return ParseTypeDeclaration();

        Advance();
        return null;
    }

    private UsingNode ParseUsing()
    {
        int startPos = CurrentToken().Position;
        var usingNode = new UsingNode
        {
            UsingKeyword = CurrentToken(),
            StartPosition = startPos
        };

        Advance();
        SkipWhitespaceAndComments();

        if (CurrentToken().Type == TokenType.Identifier)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(CurrentToken().Value);
            var firstToken = CurrentToken();
            Advance();

            while (!IsAtEnd() && CurrentToken().Type == TokenType.Dot)
            {
                sb.Append(".");
                Advance();
                SkipWhitespaceAndComments();
                if (CurrentToken().Type == TokenType.Identifier)
                {
                    sb.Append(CurrentToken().Value);
                    Advance();
                }
            }

            usingNode.NamespaceName = new SymbolNode(
                new Token(TokenType.Identifier, sb.ToString(), firstToken.Line, firstToken.Column, firstToken.Position),
                SymbolRole.NamespaceName);
        }

        SkipUntil(TokenType.Semicolon);
        if (!IsAtEnd()) Advance();

        return usingNode;
    }

    private NamespaceNode ParseNamespace()
    {
        int startPos = CurrentToken().Position;
        var nsNode = new NamespaceNode
        {
            NamespaceKeyword = CurrentToken(),
            StartPosition = startPos
        };

        Advance();
        SkipWhitespaceAndComments();

        // Parse namespace name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(CurrentToken().Value);
            var firstToken = CurrentToken();
            Advance();

            while (!IsAtEnd() && CurrentToken().Type == TokenType.Dot)
            {
                sb.Append(".");
                Advance();
                SkipWhitespaceAndComments();
                if (CurrentToken().Type == TokenType.Identifier)
                {
                    sb.Append(CurrentToken().Value);
                    Advance();
                }
            }

            nsNode.NamespaceName = new SymbolNode(
                new Token(TokenType.Identifier, sb.ToString(), firstToken.Line, firstToken.Column, firstToken.Position),
                SymbolRole.NamespaceName);
        }

        SkipWhitespaceAndComments();

        // Parse namespace body
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightBrace)
            {
                var member = ParseTopLevel();
                if (member != null)
                    nsNode.Members.Add(member);
                SkipWhitespaceAndComments();
            }

            if (!IsAtEnd())
            {
                nsNode.EndPosition = CurrentToken().Position;
                Advance();
            }
        }

        return nsNode;
    }

    private bool IsTypeDeclaration()
    {
        int saved = _current;

        // Skip modifiers
        while (!IsAtEnd() && CurrentToken().Type == TokenType.Keyword &&
               _modifiers.Contains(CurrentToken().Value))
        {
            Advance();
            SkipWhitespaceAndComments();
        }

        bool isType = !IsAtEnd() && CurrentToken().Type == TokenType.Keyword &&
                      _typeKeywords.Contains(CurrentToken().Value);

        _current = saved;
        return isType;
    }

    private AstNode? ParseTypeDeclaration()
    {
        var modifiers = new List<Token>();

        // Parse modifiers
        while (!IsAtEnd() && CurrentToken().Type == TokenType.Keyword &&
               _modifiers.Contains(CurrentToken().Value))
        {
            modifiers.Add(CurrentToken());
            Advance();
            SkipWhitespaceAndComments();
        }

        if (IsAtEnd() || CurrentToken().Type != TokenType.Keyword)
            return null;

        string typeKeyword = CurrentToken().Value;

        if (typeKeyword == "class")
            return ParseClass(modifiers);
        if (typeKeyword == "interface")
            return ParseInterface(modifiers);
        if (typeKeyword == "enum")
            return ParseEnum(modifiers);

        return null;
    }

    private ClassNode ParseClass(List<Token> modifiers)
    {
        int startPos = CurrentToken().Position;
        var classNode = new ClassNode
        {
            ClassKeyword = CurrentToken(),
            StartPosition = startPos,
            Modifiers = modifiers
        };

        Advance(); // Skip 'class'
        SkipWhitespaceAndComments();

        // Parse class name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            classNode.ClassName = new SymbolNode(CurrentToken(), SymbolRole.ClassName);
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse base types
        if (CurrentToken().Type == TokenType.Colon)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
            {
                classNode.BaseTypes.Add(new SymbolNode(CurrentToken(), SymbolRole.TypeName));
                Advance();
                SkipWhitespaceAndComments();

                if (CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
                else
                {
                    break;
                }
            }
        }

        SkipWhitespaceAndComments();

        // Parse class body
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightBrace)
            {
                var member = ParseClassMember();
                if (member != null)
                    classNode.Members.Add(member);
                SkipWhitespaceAndComments();
            }

            if (!IsAtEnd())
            {
                classNode.EndPosition = CurrentToken().Position;
                Advance();
            }
        }

        return classNode;
    }

    private InterfaceNode ParseInterface(List<Token> modifiers)
    {
        int startPos = CurrentToken().Position;
        var interfaceNode = new InterfaceNode
        {
            InterfaceKeyword = CurrentToken(),
            StartPosition = startPos,
            Modifiers = modifiers
        };

        Advance(); // Skip 'interface'
        SkipWhitespaceAndComments();

        // Parse interface name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            interfaceNode.InterfaceName = new SymbolNode(CurrentToken(), SymbolRole.InterfaceName);
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse base interfaces
        if (CurrentToken().Type == TokenType.Colon)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
            {
                interfaceNode.BaseInterfaces.Add(new SymbolNode(CurrentToken(), SymbolRole.TypeName));
                Advance();
                SkipWhitespaceAndComments();

                if (CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
                else
                {
                    break;
                }
            }
        }

        SkipWhitespaceAndComments();

        // Parse interface body
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightBrace)
            {
                var member = ParseClassMember();
                if (member != null)
                    interfaceNode.Members.Add(member);
                SkipWhitespaceAndComments();
            }

            if (!IsAtEnd())
            {
                interfaceNode.EndPosition = CurrentToken().Position;
                Advance();
            }
        }

        return interfaceNode;
    }

    private EnumNode ParseEnum(List<Token> modifiers)
    {
        int startPos = CurrentToken().Position;
        var enumNode = new EnumNode
        {
            EnumKeyword = CurrentToken(),
            StartPosition = startPos,
            Modifiers = modifiers
        };

        Advance(); // Skip 'enum'
        SkipWhitespaceAndComments();

        // Parse enum name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            enumNode.EnumName = new SymbolNode(CurrentToken(), SymbolRole.EnumName);
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse enum body
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightBrace)
            {
                if (CurrentToken().Type == TokenType.Identifier)
                {
                    enumNode.Members.Add(new SymbolNode(CurrentToken(), SymbolRole.FieldName));
                    Advance();
                    SkipWhitespaceAndComments();

                    // Skip optional value assignment
                    if (CurrentToken().Type == TokenType.Operator && CurrentToken().Value == "=")
                    {
                        SkipUntil(TokenType.Comma, TokenType.RightBrace);
                    }

                    if (CurrentToken().Type == TokenType.Comma)
                    {
                        Advance();
                        SkipWhitespaceAndComments();
                    }
                }
                else
                {
                    Advance();
                }
            }

            if (!IsAtEnd())
            {
                enumNode.EndPosition = CurrentToken().Position;
                Advance();
            }
        }

        return enumNode;
    }

    private AstNode? ParseClassMember()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
            return null;

        // Attributes
        if (CurrentToken().Type == TokenType.Attribute)
            return ParseAttribute();

        var modifiers = new List<Token>();

        // Parse modifiers
        while (!IsAtEnd() && CurrentToken().Type == TokenType.Keyword &&
               _modifiers.Contains(CurrentToken().Value))
        {
            modifiers.Add(CurrentToken());
            Advance();
            SkipWhitespaceAndComments();
        }

        if (IsAtEnd())
            return null;

        // Try to determine member type
        int saved = _current;
        Token? typeToken = null;

        if (CurrentToken().Type == TokenType.Identifier || CurrentToken().Type == TokenType.Keyword)
        {
            typeToken = CurrentToken();
            Advance();
            SkipWhitespaceAndComments();

            if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
            {
                Token nameToken = CurrentToken();
                Advance();
                SkipWhitespaceAndComments();

                if (!IsAtEnd())
                {
                    if (CurrentToken().Type == TokenType.LeftParen)
                    {
                        // Method
                        _current = saved;
                        return ParseMethod(modifiers);
                    }
                    else if (CurrentToken().Type == TokenType.LeftBrace)
                    {
                        // Property with getter/setter
                        _current = saved;
                        return ParseProperty(modifiers);
                    }
                    else if (CurrentToken().Type == TokenType.Semicolon ||
                             CurrentToken().Type == TokenType.Operator)
                    {
                        // Field
                        _current = saved;
                        return ParseField(modifiers);
                    }
                }
            }
        }

        _current = saved;
        SkipToNextMember();
        return null;
    }

    private MethodNode ParseMethod(List<Token> modifiers)
    {
        int startPos = CurrentToken().Position;
        var methodNode = new MethodNode
        {
            StartPosition = startPos,
            Modifiers = modifiers
        };

        // Parse return type
        if (CurrentToken().Type == TokenType.Identifier || CurrentToken().Type == TokenType.Keyword)
        {
            methodNode.ReturnTypeToken = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse method name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            methodNode.MethodName = new SymbolNode(CurrentToken(), SymbolRole.MethodName);
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse parameters
        if (CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                var param = ParseParameter();
                if (param != null)
                    methodNode.Parameters.Add(param);

                SkipWhitespaceAndComments();

                if (CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
            }

            if (!IsAtEnd())
                Advance(); // Skip )
        }

        SkipWhitespaceAndComments();

        // Parse body
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            var body = ParseBlock();
            if (body != null)
                methodNode.Body = body.Statements;
        }
        else if (CurrentToken().Type == TokenType.Semicolon)
        {
            Advance();
        }

        return methodNode;
    }

    private ParameterNode ParseParameter()
    {
        var paramNode = new ParameterNode
        {
            StartPosition = CurrentToken().Position
        };

        // Skip ref/out/params
        if (CurrentToken().Type == TokenType.Keyword &&
            (CurrentToken().Value == "ref" || CurrentToken().Value == "out" || CurrentToken().Value == "params"))
        {
            Advance();
            SkipWhitespaceAndComments();
        }

        // Parse type
        if (CurrentToken().Type == TokenType.Identifier || CurrentToken().Type == TokenType.Keyword)
        {
            paramNode.TypeToken = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse parameter name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            paramNode.ParameterName = new SymbolNode(CurrentToken(), SymbolRole.ParameterName);
            Advance();
        }

        // Skip default value
        SkipWhitespaceAndComments();
        if (CurrentToken().Type == TokenType.Operator && CurrentToken().Value == "=")
        {
            SkipUntil(TokenType.Comma, TokenType.RightParen);
        }

        return paramNode;
    }

    private PropertyNode ParseProperty(List<Token> modifiers)
    {
        int startPos = CurrentToken().Position;
        var propertyNode = new PropertyNode
        {
            StartPosition = startPos,
            Modifiers = modifiers
        };

        // Parse type
        if (CurrentToken().Type == TokenType.Identifier || CurrentToken().Type == TokenType.Keyword)
        {
            propertyNode.TypeToken = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse property name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            propertyNode.PropertyName = new SymbolNode(CurrentToken(), SymbolRole.PropertyName);
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse accessors
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            Advance();
            SkipUntil(TokenType.RightBrace);
            if (!IsAtEnd())
                Advance();
        }

        return propertyNode;
    }

    private FieldNode ParseField(List<Token> modifiers)
    {
        int startPos = CurrentToken().Position;
        var fieldNode = new FieldNode
        {
            StartPosition = startPos,
            Modifiers = modifiers
        };

        // Parse type
        if (CurrentToken().Type == TokenType.Identifier || CurrentToken().Type == TokenType.Keyword)
        {
            fieldNode.TypeToken = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse field name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            fieldNode.FieldName = new SymbolNode(CurrentToken(), SymbolRole.FieldName);
            Advance();
        }

        SkipUntil(TokenType.Semicolon);
        if (!IsAtEnd())
            Advance();

        return fieldNode;
    }

    private BlockNode ParseBlock()
    {
        var block = new BlockNode { StartPosition = CurrentToken().Position };

        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightBrace)
            {
                var stmt = ParseStatement();
                if (stmt != null)
                    block.Statements.Add(stmt);
                SkipWhitespaceAndComments();
            }

            if (!IsAtEnd())
            {
                block.EndPosition = CurrentToken().Position;
                Advance();
            }
        }

        return block;
    }

    private AstNode? ParseStatement()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
            return null;

        Token current = CurrentToken();

        if (current.Type == TokenType.Keyword)
        {
            switch (current.Value)
            {
                case "if":
                    return ParseIf();
                case "for":
                    return ParseFor();
                case "while":
                    return ParseWhile();
                case "return":
                    return ParseReturn();
                case "var":
                case "int":
                case "string":
                case "bool":
                case "double":
                case "float":
                case "decimal":
                case "long":
                case "short":
                case "byte":
                case "char":
                case "object":
                    return ParseVariableDeclaration();
            }
        }

        if (current.Type == TokenType.Identifier)
        {
            // Could be variable declaration or expression statement
            int saved = _current;
            Advance();
            SkipWhitespaceAndComments();

            if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
            {
                _current = saved;
                return ParseVariableDeclaration();
            }

            _current = saved;
            return ParseExpressionStatement();
        }

        return ParseExpressionStatement();
    }

    private IfStatementNode ParseIf()
    {
        int startPos = CurrentToken().Position;
        var ifNode = new IfStatementNode
        {
            IfKeyword = CurrentToken(),
            StartPosition = startPos
        };

        Advance(); // Skip 'if'
        SkipWhitespaceAndComments();

        // Parse condition
        if (CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();
            ifNode.Condition = ParseExpression();
            SkipUntil(TokenType.RightParen);
            if (!IsAtEnd()) Advance();
        }

        SkipWhitespaceAndComments();

        // Parse then body
        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            var block = ParseBlock();
            ifNode.ThenBody = block.Statements;
        }
        else
        {
            var stmt = ParseStatement();
            if (stmt != null)
                ifNode.ThenBody.Add(stmt);
        }

        SkipWhitespaceAndComments();

        // Parse else
        if (!IsAtEnd() && CurrentToken().Type == TokenType.Keyword && CurrentToken().Value == "else")
        {
            Advance();
            SkipWhitespaceAndComments();

            if (CurrentToken().Type == TokenType.LeftBrace)
            {
                var block = ParseBlock();
                ifNode.ElseBody = block.Statements;
            }
            else
            {
                var stmt = ParseStatement();
                if (stmt != null)
                    ifNode.ElseBody.Add(stmt);
            }
        }

        return ifNode;
    }

    private ForStatementNode ParseFor()
    {
        int startPos = CurrentToken().Position;
        var forNode = new ForStatementNode
        {
            ForKeyword = CurrentToken(),
            StartPosition = startPos
        };

        Advance(); // Skip 'for'
        SkipWhitespaceAndComments();

        if (CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipUntil(TokenType.RightParen);
            if (!IsAtEnd()) Advance();
        }

        SkipWhitespaceAndComments();

        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            var block = ParseBlock();
            forNode.Body = block.Statements;
        }

        return forNode;
    }

    private WhileStatementNode ParseWhile()
    {
        int startPos = CurrentToken().Position;
        var whileNode = new WhileStatementNode
        {
            WhileKeyword = CurrentToken(),
            StartPosition = startPos
        };

        Advance(); // Skip 'while'
        SkipWhitespaceAndComments();

        if (CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();
            whileNode.Condition = ParseExpression();
            SkipUntil(TokenType.RightParen);
            if (!IsAtEnd()) Advance();
        }

        SkipWhitespaceAndComments();

        if (CurrentToken().Type == TokenType.LeftBrace)
        {
            var block = ParseBlock();
            whileNode.Body = block.Statements;
        }

        return whileNode;
    }

    private ReturnStatementNode ParseReturn()
    {
        int startPos = CurrentToken().Position;
        var returnNode = new ReturnStatementNode
        {
            ReturnKeyword = CurrentToken(),
            StartPosition = startPos
        };

        Advance(); // Skip 'return'
        SkipWhitespaceAndComments();

        if (CurrentToken().Type != TokenType.Semicolon)
        {
            returnNode.Expression = ParseExpression();
        }

        SkipUntil(TokenType.Semicolon);
        if (!IsAtEnd()) Advance();

        return returnNode;
    }

    private VariableDeclarationNode ParseVariableDeclaration()
    {
        int startPos = CurrentToken().Position;
        var varNode = new VariableDeclarationNode
        {
            StartPosition = startPos
        };

        // Parse type
        if (CurrentToken().Type == TokenType.Keyword || CurrentToken().Type == TokenType.Identifier)
        {
            varNode.TypeToken = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse variable name
        if (CurrentToken().Type == TokenType.Identifier)
        {
            varNode.VariableName = new SymbolNode(CurrentToken(), SymbolRole.LocalVariable);
            Advance();
        }

        SkipWhitespaceAndComments();

        // Parse initializer
        if (CurrentToken().Type == TokenType.Operator && CurrentToken().Value == "=")
        {
            Advance();
            SkipWhitespaceAndComments();
            varNode.Initializer = ParseExpression();
        }

        SkipUntil(TokenType.Semicolon);
        if (!IsAtEnd()) Advance();

        return varNode;
    }

    private AstNode ParseExpressionStatement()
    {
        var expr = ParseExpression();
        SkipUntil(TokenType.Semicolon);
        if (!IsAtEnd()) Advance();
        return expr;
    }

    private AstNode? ParseExpression()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
            return null;

        Token current = CurrentToken();

        // Literals
        if (current.Type == TokenType.String || current.Type == TokenType.Number ||
            current.Type == TokenType.Character || current.Type == TokenType.Boolean ||
            current.Type == TokenType.Null)
        {
            var literal = new LiteralNode(current);
            Advance();
            return literal;
        }

        // Identifiers
        if (current.Type == TokenType.Identifier)
        {
            var symbol = new SymbolNode(current, SymbolRole.Unknown);
            Advance();
            SkipWhitespaceAndComments();

            // Method call
            if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
            {
                var call = new MethodCallNode
                {
                    MethodName = new SymbolNode(symbol.Token, SymbolRole.MethodCall),
                    StartPosition = symbol.StartPosition
                };

                Advance(); // Skip (
                SkipWhitespaceAndComments();

                while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
                {
                    var arg = ParseExpression();
                    if (arg != null)
                        call.Arguments.Add(arg);

                    SkipWhitespaceAndComments();

                    if (CurrentToken().Type == TokenType.Comma)
                    {
                        Advance();
                        SkipWhitespaceAndComments();
                    }
                }

                if (!IsAtEnd())
                {
                    call.EndPosition = CurrentToken().Position;
                    Advance();
                }

                return call;
            }

            return symbol;
        }

        // Skip other tokens
        Advance();
        return null;
    }

    private AttributeNode ParseAttribute()
    {
        int startPos = CurrentToken().Position;
        string attrText = CurrentToken().Value;

        var attrNode = new AttributeNode
        {
            StartPosition = startPos
        };

        // Extract attribute name (simplified)
        if (attrText.Length > 2)
        {
            string inner = attrText.Substring(1, attrText.Length - 2).Trim();
            int parenIndex = inner.IndexOf('(');
            string name = parenIndex > 0 ? inner.Substring(0, parenIndex).Trim() : inner;

            attrNode.AttributeName = new SymbolNode(
                new Token(TokenType.Identifier, name, CurrentToken().Line, CurrentToken().Column, startPos),
                SymbolRole.AttributeName);
        }

        Advance();
        return attrNode;
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd() &&
               (CurrentToken().Type == TokenType.Whitespace ||
                CurrentToken().Type == TokenType.Comment ||
                CurrentToken().Type == TokenType.Preprocessor))
        {
            Advance();
        }
    }

    private void SkipUntil(params TokenType[] types)
    {
        while (!IsAtEnd() && !types.Contains(CurrentToken().Type))
        {
            Advance();
        }
    }

    private void SkipToNextMember()
    {
        int braceCount = 0;
        while (!IsAtEnd())
        {
            if (CurrentToken().Type == TokenType.LeftBrace)
                braceCount++;
            if (CurrentToken().Type == TokenType.RightBrace)
            {
                if (braceCount > 0)
                    braceCount--;
                else
                    break;
            }

            if (CurrentToken().Type == TokenType.Semicolon && braceCount == 0)
            {
                Advance();
                break;
            }

            Advance();
        }
    }

    private Token CurrentToken()
    {
        return _tokens[_current];
    }

    private void Advance()
    {
        if (!IsAtEnd())
            _current++;
    }

    private bool IsAtEnd()
    {
        return _current >= _tokens.Count || CurrentToken().Type == TokenType.EOF;
    }
}