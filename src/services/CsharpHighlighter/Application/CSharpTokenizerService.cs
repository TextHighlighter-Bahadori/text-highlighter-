using System.Text;
using CsharpHighlighter.Application;
using CsharpHighlighter.Domain.Token;

namespace CSharpHighlighter.Application;

public class CSharpTokenizerService : ICSharpTokenizerService
{
    private readonly string _input;
    private int _position;
    private int _line;
    private int _column;

    private static readonly HashSet<string> Keywords = new()
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
        "char", "checked", "class", "const", "continue", "decimal", "default",
        "delegate", "do", "double", "else", "enum", "event", "explicit",
        "extern", "false", "finally", "fixed", "float", "for", "foreach",
        "goto", "if", "implicit", "in", "int", "interface", "internal",
        "is", "lock", "long", "namespace", "new", "null", "object",
        "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch",
        "this", "throw", "true", "try", "typeof", "uint", "ulong",
        "unchecked", "unsafe", "ushort", "using", "virtual", "void",
        "volatile", "while", "add", "alias", "ascending", "async", "await",
        "by", "descending", "dynamic", "equals", "from", "get", "global",
        "group", "into", "join", "let", "nameof", "on", "orderby",
        "partial", "remove", "select", "set", "value", "var", "when", "where", "yield"
    };

    private static readonly HashSet<string> Operators = new()
    {
        "+", "-", "*", "/", "%", "=", "==", "!=", "<", ">", "<=", ">=",
        "&&", "||", "!", "&", "|", "^", "~", "<<", ">>", "++", "--",
        "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
        "??", "?.", "=>", "??"
    };

    public CSharpTokenizerService(string input)
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

    private Token? NextToken()
    {
        if (_position >= _input.Length)
            return null;

        char current = _input[_position];

        // Whitespace
        if (char.IsWhiteSpace(current))
            return ReadWhitespace();

        // Comments
        if (current == '/' && Peek() == '/')
            return ReadSingleLineComment();
        if (current == '/' && Peek() == '*')
            return ReadMultiLineComment();

        // Preprocessor directives
        if (current == '#')
            return ReadPreprocessor();

        // Attributes
        if (current == '[')
        {
            if (IsAttribute())
                return ReadAttribute();
            return CreateToken(TokenType.LeftBracket, "[");
        }

        // Strings
        if (current == '"')
            return ReadString();
        if (current == '@' && Peek() == '"')
            return ReadVerbatimString();
        if (current == '$' && Peek() == '"')
            return ReadInterpolatedString();

        // Characters
        if (current == '\'')
            return ReadCharacter();

        // Delimiters
        if (current == '(')
            return CreateToken(TokenType.LeftParen, "(");
        if (current == ')')
            return CreateToken(TokenType.RightParen, ")");
        if (current == '{')
            return CreateToken(TokenType.LeftBrace, "{");
        if (current == '}')
            return CreateToken(TokenType.RightBrace, "}");
        if (current == ']')
            return CreateToken(TokenType.RightBracket, "]");
        if (current == ';')
            return CreateToken(TokenType.Semicolon, ";");
        if (current == ',')
            return CreateToken(TokenType.Comma, ",");
        if (current == ':')
            return CreateToken(TokenType.Colon, ":");
        
        if (char.IsDigit(current))
            return ReadNumber();

        var op = ReadOperator();
        if (op != null)
            return op;

        if (char.IsLetter(current) || current == '_' || current == '@')
            return ReadIdentifierOrKeyword();

        if (current == '.')
            return CreateToken(TokenType.Dot, ".");

        return CreateToken(TokenType.Unknown, current.ToString());
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

    private Token ReadSingleLineComment()
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

    private Token ReadMultiLineComment()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        _position += 2; 
        _column += 2;

        while (_position < _input.Length - 1)
        {
            if (_input[_position] == '*' && _input[_position + 1] == '/')
            {
                _position += 2;
                _column += 2;
                break;
            }

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

        return new Token(TokenType.Comment, _input.Substring(start, _position - start), startLine, startColumn, start);
    }

    private Token ReadPreprocessor()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        while (_position < _input.Length && _input[_position] != '\n')
        {
            _position++;
            _column++;
        }

        return new Token(TokenType.Preprocessor, _input.Substring(start, _position - start), startLine, startColumn, start);
    }

    private bool IsAttribute()
    {
        int lookahead = _position + 1;
        while (lookahead < _input.Length)
        {
            char c = _input[lookahead];
            if (c == ']')
                return true;
            if (c == ';' || c == '{' || c == '\n')
                return false;
            lookahead++;
        }
        return false;
    }

    private Token ReadAttribute()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        while (_position < _input.Length && _input[_position] != ']')
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

        if (_position < _input.Length)
        {
            _position++; 
            _column++;
        }

        return new Token(TokenType.Attribute, _input.Substring(start, _position - start), startLine, startColumn, start);
    }

    private Token ReadString()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        _position++; 
        _column++;

        var sb = new StringBuilder("\"");

        while (_position < _input.Length)
        {
            char c = _input[_position];

            if (c == '"')
            {
                sb.Append(c);
                _position++;
                _column++;
                break;
            }

            if (c == '\\' && _position + 1 < _input.Length)
            {
                sb.Append(c);
                _position++;
                _column++;
                sb.Append(_input[_position]);
            }
            else
            {
                sb.Append(c);
                if (c == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
            }
            _position++;
        }

        return new Token(TokenType.String, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadVerbatimString()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        _position += 2; // Skip @"
        _column += 2;

        var sb = new StringBuilder("@\"");

        while (_position < _input.Length)
        {
            char c = _input[_position];

            if (c == '"')
            {
                sb.Append(c);
                _position++;
                _column++;

              
                if (_position < _input.Length && _input[_position] == '"')
                {
                    sb.Append('"');
                    _position++;
                    _column++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                sb.Append(c);
                if (c == '\n')
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

        return new Token(TokenType.String, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadInterpolatedString()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        _position += 2; 
        _column += 2;

        var sb = new StringBuilder("$\"");

        while (_position < _input.Length)
        {
            char c = _input[_position];

            if (c == '"')
            {
                sb.Append(c);
                _position++;
                _column++;
                break;
            }

            if (c == '\\' && _position + 1 < _input.Length)
            {
                sb.Append(c);
                _position++;
                _column++;
                sb.Append(_input[_position]);
            }
            else
            {
                sb.Append(c);
                if (c == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
            }
            _position++;
        }

        return new Token(TokenType.String, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadCharacter()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        _position++; 
        _column++;

        var sb = new StringBuilder("'");

        while (_position < _input.Length)
        {
            char c = _input[_position];

            if (c == '\'')
            {
                sb.Append(c);
                _position++;
                _column++;
                break;
            }

            if (c == '\\' && _position + 1 < _input.Length)
            {
                sb.Append(c);
                _position++;
                _column++;
                sb.Append(_input[_position]);
            }
            else
            {
                sb.Append(c);
            }

            _position++;
            _column++;
        }

        return new Token(TokenType.Character, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadNumber()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        var sb = new StringBuilder();

        // Hex numbers
        if (_input[_position] == '0' && _position + 1 < _input.Length &&
            (_input[_position + 1] == 'x' || _input[_position + 1] == 'X'))
        {
            sb.Append(_input[_position]);
            sb.Append(_input[_position + 1]);
            _position += 2;
            _column += 2;

            while (_position < _input.Length && (char.IsDigit(_input[_position]) ||
                   "abcdefABCDEF".Contains(_input[_position])))
            {
                sb.Append(_input[_position]);
                _position++;
                _column++;
            }
        }
        else
        {
              while (_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == '.'))
            {
                sb.Append(_input[_position]);
                _position++;
                _column++;
            }

            
            if (_position < _input.Length && (_input[_position] == 'e' || _input[_position] == 'E'))
            {
                sb.Append(_input[_position]);
                _position++;
                _column++;

                if (_position < _input.Length && (_input[_position] == '+' || _input[_position] == '-'))
                {
                    sb.Append(_input[_position]);
                    _position++;
                    _column++;
                }

                while (_position < _input.Length && char.IsDigit(_input[_position]))
                {
                    sb.Append(_input[_position]);
                    _position++;
                    _column++;
                }
            }
        }
        
        if (_position < _input.Length && "fFdDmMlLuU".Contains(_input[_position]))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }

        return new Token(TokenType.Number, sb.ToString(), startLine, startColumn, start);
    }

    private Token? ReadOperator()
    {
        foreach (var op in Operators.OrderByDescending(o => o.Length))
        {
            if (_position + op.Length <= _input.Length &&
                _input.Substring(_position, op.Length) == op)
            {
                return CreateToken(TokenType.Operator, op);
            }
        }
        return null;
    }

    private Token ReadIdentifierOrKeyword()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        var sb = new StringBuilder();
        
        if (_input[_position] == '@')
        {
            sb.Append('@');
            _position++;
            _column++;
        }

        while (_position < _input.Length &&
               (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }

        string value = sb.ToString();
        string normalizedValue = value.TrimStart('@');

        if (normalizedValue == "true" || normalizedValue == "false")
            return new Token(TokenType.Boolean, value, startLine, startColumn, start);
        if (normalizedValue == "null")
            return new Token(TokenType.Null, value, startLine, startColumn, start);
        if (Keywords.Contains(normalizedValue))
            return new Token(TokenType.Keyword, value, startLine, startColumn, start);

        return new Token(TokenType.Identifier, value, startLine, startColumn, start);
    }

    private char Peek()
    {
        if (_position + 1 < _input.Length)
            return _input[_position + 1];
        return '\0';
    }

    private Token CreateToken(TokenType type, string value)
    {
        var token = new Token(type, value, _line, _column, _position);
        _position += value.Length;
        _column += value.Length;
        return token;
    }
}