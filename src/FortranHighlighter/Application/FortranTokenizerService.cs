using System.Text;
using FortranHighlighter.Domain.Token;

namespace FortranHighlighter.Application;

public class FortranTokenizerService : IFortranTokenizerService
{
    private readonly string _input;
    private int _position;
    private int _line;
    private int _column;

    private static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "program", "subroutine", "function", "end", "enddo", "endif", "endselect",
        "integer", "real", "character", "logical", "complex", "double",
        "dimension", "parameter", "implicit", "none",
        "do", "while", "if", "then", "else", "elseif", "select", "case",
        "call", "return", "continue", "stop", "exit",
        "read", "write", "print", "open", "close", "format",
        "and", "or", "not", "eqv", "neqv",
        "true", "false"
    };

    public FortranTokenizerService(string input)
    {
        _input = input;
        _position = 0;
        _line = 1;
        _column = 1;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _input.Length)
        {
            var token = NextToken();
            if (token != null)
            {
                tokens.Add(token);
            }
        }

        tokens.Add(new Token(TokenType.EOF, "", _line, _column, _position));
        return tokens;
    }

    private Token NextToken()
    {
        if (_position >= _input.Length)
            return null;

        char current = _input[_position];

        // Whitespace
        if (char.IsWhiteSpace(current))
            return ReadWhitespace();

        // Comments (! or C/c at start of line)
        if (current == '!' || (_column == 1 && (current == 'C' || current == 'c')))
            return ReadComment();

        // String literals
        if (current == '\'' || current == '"')
            return ReadString(current);

        // Numbers
        if (char.IsDigit(current))
            return ReadNumber();

        // Operators and delimiters
        if (current == '(')
            return CreateToken(TokenType.LeftParen, "(");
        if (current == ')')
            return CreateToken(TokenType.RightParen, ")");
        if (current == ',')
            return CreateToken(TokenType.Comma, ",");
        if (current == '+')
            return CreateToken(TokenType.Plus, "+");
        if (current == '-')
            return CreateToken(TokenType.Minus, "-");
        if (current == '*')
        {
            if (Peek() == '*')
            {
                Advance();
                return CreateToken(TokenType.Power, "**");
            }
            return CreateToken(TokenType.Multiply, "*");
        }
        if (current == '/')
        {
            if (Peek() == '=')
            {
                Advance();
                return CreateToken(TokenType.NotEquals, "/=");
            }
            return CreateToken(TokenType.Divide, "/");
        }
        if (current == '=')
        {
            if (Peek() == '=')
            {
                Advance();
                return CreateToken(TokenType.Equals, "==");
            }
            return CreateToken(TokenType.Assignment, "=");
        }
        if (current == '<')
        {
            if (Peek() == '=')
            {
                Advance();
                return CreateToken(TokenType.LessOrEqual, "<=");
            }
            return CreateToken(TokenType.LessThan, "<");
        }
        if (current == '>')
        {
            if (Peek() == '=')
            {
                Advance();
                return CreateToken(TokenType.GreaterOrEqual, ">=");
            }
            return CreateToken(TokenType.GreaterThan, ">");
        }
        if (current == ':')
        {
            if (Peek() == ':')
            {
                Advance();
                return CreateToken(TokenType.DoubleColon, "::");
            }
            return CreateToken(TokenType.Colon, ":");
        }

        // Identifiers and keywords
        if (char.IsLetter(current) || current == '_')
            return ReadIdentifierOrKeyword();

        // Label numbers at start of line
        if (_column <= 6 && char.IsDigit(current))
            return ReadLabel();

        // Unknown character - skip
        Advance();
        return null;
    }

    private Token ReadWhitespace()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            if (_input[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            _position++;
        }

        return new Token(TokenType.Whitespace, _input.Substring(start, _position - start), startLine, startColumn, start);
    }

    private Token ReadComment()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        while (_position < _input.Length && _input[_position] != '\n')
        {
            _position++;
            _column++;
        }

        return new Token(TokenType.Comment, _input.Substring(start, _position - start), startLine, startColumn, start);
    }

    private Token ReadString(char quote)
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        _position++;
        _column++;

        var sb = new StringBuilder();
        sb.Append(quote);

        while (_position < _input.Length)
        {
            char c = _input[_position];

            if (c == quote)
            {
                if (Peek() == quote) // Escaped quote
                {
                    sb.Append(c);
                    sb.Append(c);
                    _position += 2;
                    _column += 2;
                }
                else
                {
                    sb.Append(c);
                    _position++;
                    _column++;
                    break;
                }
            }
            else
            {
                sb.Append(c);
                _position++;
                _column++;
            }
        }

        return new Token(TokenType.String, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadNumber()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        var sb = new StringBuilder();
        bool isReal = false;

        while (_position < _input.Length)
        {
            char c = _input[_position];
            
            if (char.IsDigit(c))
            {
                sb.Append(c);
                _position++;
                _column++;
            }
            else if (c == '.' && !isReal)
            {
                isReal = true;
                sb.Append(c);
                _position++;
                _column++;
            }
            else if ((c == 'e' || c == 'E' || c == 'd' || c == 'D') && _position + 1 < _input.Length)
            {
                isReal = true;
                sb.Append(c);
                _position++;
                _column++;
                
                if (_position < _input.Length && (_input[_position] == '+' || _input[_position] == '-'))
                {
                    sb.Append(_input[_position]);
                    _position++;
                    _column++;
                }
            }
            else
            {
                break;
            }
        }

        return new Token(isReal ? TokenType.Real : TokenType.Integer, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadIdentifierOrKeyword()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        var sb = new StringBuilder();

        while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }

        string value = sb.ToString();
        string lowerValue = value.ToLower();

        TokenType type = lowerValue switch
        {
            "program" => TokenType.Program,
            "subroutine" => TokenType.Subroutine,
            "function" => TokenType.Function,
            "end" => TokenType.End,
            "enddo" => TokenType.EndDo,
            "endif" => TokenType.EndIf,
            "endselect" => TokenType.EndSelect,
            "integer" => TokenType.Integer_Keyword,
            "real" => TokenType.Real_Keyword,
            "character" => TokenType.Character_Keyword,
            "logical" => TokenType.Logical_Keyword,
            "dimension" => TokenType.Dimension,
            "parameter" => TokenType.Parameter,
            "implicit" => TokenType.Implicit,
            "none" => TokenType.None,
            "do" => TokenType.Do,
            "if" => TokenType.If,
            "then" => TokenType.Then,
            "else" => TokenType.Else,
            "elseif" => TokenType.ElseIf,
            "select" => TokenType.Select,
            "case" => TokenType.Case,
            "call" => TokenType.Call,
            "return" => TokenType.Return,
            "continue" => TokenType.Continue,
            "stop" => TokenType.Stop,
            "read" => TokenType.Read,
            "write" => TokenType.Write,
            "print" => TokenType.Print,
            "open" => TokenType.Open,
            "close" => TokenType.Close,
            "and" or ".and." => TokenType.And,
            "or" or ".or." => TokenType.Or,
            "not" or ".not." => TokenType.Not,
            "true" or ".true." => TokenType.Logical,
            "false" or ".false." => TokenType.Logical,
            _ => TokenType.Identifier
        };

        return new Token(type, value, startLine, startColumn, start);
    }

    private Token ReadLabel()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        var sb = new StringBuilder();

        while (_position < _input.Length && char.IsDigit(_input[_position]))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }

        return new Token(TokenType.Label, sb.ToString(), startLine, startColumn, start);
    }

    private char Peek()
    {
        if (_position + 1 < _input.Length)
            return _input[_position + 1];
        return '\0';
    }

    private void Advance()
    {
        if (_position < _input.Length)
        {
            if (_input[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            _position++;
        }
    }

    private Token CreateToken(TokenType type, string value)
    {
        var token = new Token(type, value, _line, _column, _position);
        _position += value.Length;
        _column += value.Length;
        return token;
    }
}