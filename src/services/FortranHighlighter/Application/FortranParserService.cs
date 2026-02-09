using FortranHighlighter.Domain;
using FortranHighlighter.Domain.ASTNode;
using FortranHighlighter.Domain.Token;

namespace FortranHighlighter.Application;

public class FortranParserService : IFortranParserService
{
    private List<Token> _tokens;
    private int _current;

    public FortranParserService(List<Token> tokens)
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
                var node = ParseStatement();
                if (node != null)
                    nodes.Add(node);
            }
        }

        return nodes;
    }

    private AstNode? ParseStatement()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
            return null;

        Token current = CurrentToken();

        return current.Type switch
        {
            TokenType.Program => ParseProgram(),
            TokenType.Subroutine => ParseSubroutine(),
            TokenType.Function => ParseFunction(),
            TokenType.Integer_Keyword or TokenType.Real_Keyword or 
            TokenType.Character_Keyword or TokenType.Logical_Keyword => ParseVariableDeclaration(),
            TokenType.Do => ParseDoLoop(),
            TokenType.If => ParseIfStatement(),
            TokenType.Call => ParseCallStatement(),
            TokenType.Identifier => ParseAssignmentOrFunctionCall(),
            _ => ParseExpression()
        };
    }

    private ProgramNode ParseProgram()
    {
        int startPos = CurrentToken().Position;
        Token programKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var programNode = new ProgramNode
        {
            ProgramKeyword = programKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
        {
            programNode.ProgramName = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        while (!IsAtEnd() && !IsEndStatement())
        {
            var stmt = ParseStatement();
            if (stmt != null)
            {
                if (IsDeclaration(stmt))
                    programNode.Declarations.Add(stmt);
                else
                    programNode.Body.Add(stmt);
            }
            SkipWhitespaceAndComments();
        }

        if (!IsAtEnd())
        {
            programNode.EndPosition = CurrentToken().Position;
            SkipEndStatement();
        }

        return programNode;
    }

    private SubroutineNode ParseSubroutine()
    {
        int startPos = CurrentToken().Position;
        Token subroutineKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var subroutineNode = new SubroutineNode
        {
            SubroutineKeyword = subroutineKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
        {
            subroutineNode.SubroutineName = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                if (CurrentToken().Type == TokenType.Identifier)
                {
                    subroutineNode.Parameters.Add(CurrentToken());
                }
                Advance();
                SkipWhitespaceAndComments();
                
                if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
            }

            if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
                Advance();
        }

        SkipWhitespaceAndComments();

        while (!IsAtEnd() && !IsEndStatement())
        {
            var stmt = ParseStatement();
            if (stmt != null)
            {
                if (IsDeclaration(stmt))
                    subroutineNode.Declarations.Add(stmt);
                else
                    subroutineNode.Body.Add(stmt);
            }
            SkipWhitespaceAndComments();
        }

        if (!IsAtEnd())
        {
            subroutineNode.EndPosition = CurrentToken().Position;
            SkipEndStatement();
        }

        return subroutineNode;
    }

    private FunctionNode ParseFunction()
    {
        int startPos = CurrentToken().Position;
        Token functionKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var functionNode = new FunctionNode
        {
            FunctionKeyword = functionKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
        {
            functionNode.FunctionName = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                if (CurrentToken().Type == TokenType.Identifier)
                {
                    functionNode.Parameters.Add(CurrentToken());
                }
                Advance();
                SkipWhitespaceAndComments();
                
                if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
            }

            if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
                Advance();
        }

        SkipWhitespaceAndComments();

        while (!IsAtEnd() && !IsEndStatement())
        {
            var stmt = ParseStatement();
            if (stmt != null)
            {
                if (IsDeclaration(stmt))
                    functionNode.Declarations.Add(stmt);
                else
                    functionNode.Body.Add(stmt);
            }
            SkipWhitespaceAndComments();
        }

        if (!IsAtEnd())
        {
            functionNode.EndPosition = CurrentToken().Position;
            SkipEndStatement();
        }

        return functionNode;
    }

    private VariableDeclarationNode ParseVariableDeclaration()
    {
        int startPos = CurrentToken().Position;
        Token typeKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var declNode = new VariableDeclarationNode
        {
            TypeKeyword = typeKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.DoubleColon)
        {
            Advance();
            SkipWhitespaceAndComments();
        }

        while (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
        {
            declNode.Variables.Add(CurrentToken());
            Advance();
            SkipWhitespaceAndComments();

            if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
            {
                Advance();
                SkipWhitespaceAndComments();
                
                while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
                {
                    Advance();
                }
                
                if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
                    Advance();
                SkipWhitespaceAndComments();
            }

            if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
            {
                Advance();
                SkipWhitespaceAndComments();
            }
            else
            {
                break;
            }
        }

        declNode.EndPosition = _current < _tokens.Count ? CurrentToken().Position : startPos;
        return declNode;
    }

    private DoLoopNode ParseDoLoop()
    {
        int startPos = CurrentToken().Position;
        Token doKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var doNode = new DoLoopNode
        {
            DoKeyword = doKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
        {
            doNode.LoopVariable = CurrentToken();
            Advance();
            SkipWhitespaceAndComments();

            if (!IsAtEnd() && CurrentToken().Type == TokenType.Assignment)
            {
                Advance();
                SkipWhitespaceAndComments();

                doNode.StartValue = ParseExpression();
                SkipWhitespaceAndComments();

                if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                    doNode.EndValue = ParseExpression();
                    SkipWhitespaceAndComments();

                    if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                    {
                        Advance();
                        SkipWhitespaceAndComments();
                        doNode.StepValue = ParseExpression();
                        SkipWhitespaceAndComments();
                    }
                }
            }
        }

        while (!IsAtEnd() && CurrentToken().Type != TokenType.EndDo && !IsEndStatement())
        {
            var stmt = ParseStatement();
            if (stmt != null)
                doNode.Body.Add(stmt);
            SkipWhitespaceAndComments();
        }

        if (!IsAtEnd() && (CurrentToken().Type == TokenType.EndDo || IsEndStatement()))
        {
            doNode.EndPosition = CurrentToken().Position;
            Advance();
        }

        return doNode;
    }

    private IfStatementNode ParseIfStatement()
    {
        int startPos = CurrentToken().Position;
        Token ifKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var ifNode = new IfStatementNode
        {
            IfKeyword = ifKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();
            ifNode.Condition = ParseExpression();
            SkipWhitespaceAndComments();
            
            if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
                Advance();
        }

        SkipWhitespaceAndComments();

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Then)
            Advance();

        SkipWhitespaceAndComments();

        while (!IsAtEnd() && CurrentToken().Type != TokenType.Else && 
               CurrentToken().Type != TokenType.ElseIf && CurrentToken().Type != TokenType.EndIf)
        {
            var stmt = ParseStatement();
            if (stmt != null)
                ifNode.ThenBlock.Add(stmt);
            SkipWhitespaceAndComments();
        }

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Else)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.EndIf)
            {
                var stmt = ParseStatement();
                if (stmt != null)
                    ifNode.ElseBlock.Add(stmt);
                SkipWhitespaceAndComments();
            }
        }

        if (!IsAtEnd() && CurrentToken().Type == TokenType.EndIf)
        {
            ifNode.EndPosition = CurrentToken().Position;
            Advance();
        }

        return ifNode;
    }

    private CallStatementNode ParseCallStatement()
    {
        int startPos = CurrentToken().Position;
        Token callKeyword = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        var callNode = new CallStatementNode
        {
            CallKeyword = callKeyword,
            StartPosition = startPos
        };

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Identifier)
        {
            callNode.SubroutineName = CurrentToken();
            Advance();
        }

        SkipWhitespaceAndComments();

        if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                AstNode? arg = ParseExpression();
                if (arg != null)
                    callNode.Arguments.Add(arg);
                SkipWhitespaceAndComments();

                if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
            }

            if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
            {
                callNode.EndPosition = CurrentToken().Position;
                Advance();
            }
        }

        return callNode;
    }

    private AstNode ParseAssignmentOrFunctionCall()
    {
        int startPos = CurrentToken().Position;
        Token identifier = CurrentToken();
        Advance();
        SkipWhitespaceAndComments();

        if (!IsAtEnd() && CurrentToken().Type == TokenType.Assignment)
        {
            Token assignOp = CurrentToken();
            Advance();
            SkipWhitespaceAndComments();

            return new AssignmentNode
            {
                Variable = identifier,
                AssignmentOp = assignOp,
                Expression = ParseExpression(),
                StartPosition = startPos
            };
        }
        else if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
        {
            Advance();
            SkipWhitespaceAndComments();

            var functionCall = new FunctionCallNode
            {
                FunctionName = identifier,
                StartPosition = startPos
            };

            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                var arg = ParseExpression();
                if (arg != null)
                    functionCall.Arguments.Add(arg);
                SkipWhitespaceAndComments();

                if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                {
                    Advance();
                    SkipWhitespaceAndComments();
                }
            }

            if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
            {
                functionCall.EndPosition = CurrentToken().Position;
                Advance();
            }

            return functionCall;
        }

        return new IdentifierNode(identifier, IdentifierRole.Unknown);
    }

    private AstNode? ParseExpression()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
            return null;

        Token current = CurrentToken();

        switch (current.Type)
        {
            case TokenType.Integer:
            case TokenType.Real:
            case TokenType.String:
            case TokenType.Logical:
                var lit = new LiteralNode(current);
                Advance();
                return lit;

            case TokenType.Identifier:
                Token id = current;
                Advance();
                SkipWhitespaceAndComments();

                if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftParen)
                {
                    Advance();
                    SkipWhitespaceAndComments();

                    var funcCall = new FunctionCallNode
                    {
                        FunctionName = id,
                        StartPosition = id.Position
                    };

                    while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
                    {
                        var arg = ParseExpression();
                        if (arg != null)
                            funcCall.Arguments.Add(arg);
                        SkipWhitespaceAndComments();

                        if (!IsAtEnd() && CurrentToken().Type == TokenType.Comma)
                        {
                            Advance();
                            SkipWhitespaceAndComments();
                        }
                    }

                    if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
                    {
                        funcCall.EndPosition = CurrentToken().Position;
                        Advance();
                    }

                    return funcCall;
                }

                return new IdentifierNode(id, IdentifierRole.Unknown);

            case TokenType.LeftParen:
                Advance();
                var expr = ParseExpression();
                SkipWhitespaceAndComments();
                if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
                    Advance();
                return expr;

            default:
                Advance();
                return null;
        }
    }

    private bool IsDeclaration(AstNode node)
    {
        return node is VariableDeclarationNode;
    }

    private bool IsEndStatement()
    {
        return !IsAtEnd() && CurrentToken().Type == TokenType.End;
    }

    private void SkipEndStatement()
    {
        if (!IsAtEnd() && CurrentToken().Type == TokenType.End)
        {
            Advance();
            SkipWhitespaceAndComments();
            
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.Program || 
                               CurrentToken().Type == TokenType.Subroutine ||
                               CurrentToken().Type == TokenType.Function))
            {
                Advance();
            }
        }
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd() && (CurrentToken().Type == TokenType.Whitespace || 
                              CurrentToken().Type == TokenType.Comment))
        {
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