using System.Text;
using ClojureHighlighter.Domain;

namespace ClojureHighlighter.Application;

public class ClojureTokenizerService
{
    private readonly string _input;
    private int _position;
    private int _line;
    private int _column;

    private static readonly HashSet<string> SpecialForms = new HashSet<string>
    {
        "def", "defn", "defn-", "defmacro", "defmethod", "defmulti",
        "defprotocol", "defrecord", "defstruct", "deftype",
        "let", "letfn", "if", "if-not", "if-let", "when", "when-not",
        "when-let", "cond", "condp", "case",
        "do", "loop", "recur", "fn", "throw", "try", "catch", "finally",
        "quote", "var", "new", "set!", ".", "..", "->", "->>",
        "ns", "in-ns", "import", "require", "use", "refer",
        "and", "or", "not"
    };


    public ClojureTokenizerService(string input)
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

            // Comments
            if (current == ';')
                return ReadComment();

            // Strings
            if (current == '"')
                return ReadString();

            // Characters
            if (current == '\\')
                return ReadCharacter();

            // Keywords
            if (current == ':')
                return ReadKeyword();

            // Reader macros
            if (current == '\'')
                return CreateToken(TokenType.Quote, "'");
            if (current == '@')
                return CreateToken(TokenType.Deref, "@");
            if (current == '^')
                return CreateToken(TokenType.Metadata, "^");
            if (current == '`')
                return CreateToken(TokenType.SyntaxQuote, "`");
            if (current == '~')
            {
                if (Peek() == '@')
                {
                    Advance();
                    return CreateToken(TokenType.UnquoteSplicing, "~@");
                }
                return CreateToken(TokenType.Unquote, "~");
            }

            // Dispatch reader macro
            if (current == '#')
                return ReadDispatch();

            // Delimiters
            if (current == '(')
                return CreateToken(TokenType.LeftParen, "(");
            if (current == ')')
                return CreateToken(TokenType.RightParen, ")");
            if (current == '[')
                return CreateToken(TokenType.LeftBracket, "[");
            if (current == ']')
                return CreateToken(TokenType.RightBracket, "]");
            if (current == '{')
                return CreateToken(TokenType.LeftBrace, "{");
            if (current == '}')
                return CreateToken(TokenType.RightBrace, "}");

            // Numbers
            if (char.IsDigit(current) || (current == '-' && _position + 1 < _input.Length && char.IsDigit(_input[_position + 1])))
                return ReadNumber();
            
            if (IsSymbolStart(current))
                return ReadSymbol();

            // Unknown characters
            return CreateToken(TokenType.Symbol, current.ToString());
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

        return new Token(TokenType.Whitespace, _input.Substring(start, _position - start), startLine, startColumn,
            start);
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

    private Token ReadString()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        // Skip opening quote
        _position++;
        _column++;

        var stringLiteral = new StringBuilder("\"");

        while (_position < _input.Length)
        {
            char nextChar = _input[_position];

            //it is for closing quote
            if (nextChar == '"')
            {
                stringLiteral.Append(nextChar);
                _position++;
                _column++;
                break;
            }

            // it is for handling escape sequences.
            if (nextChar == '\\' && _position + 1 < _input.Length)
            {
                stringLiteral.Append(nextChar);
                _position++;
                _column++;
                stringLiteral.Append(_input[_position]);
            }
            else
            {
                stringLiteral.Append(nextChar);
                if (nextChar == '\n')
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

        return new Token(TokenType.String, stringLiteral.ToString(), startLine, startColumn, start);
    }

    private Token ReadCharacter()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        // Skip backslash
        _position++;
        _column++;

        if (_position >= _input.Length)
            return new Token(TokenType.Character, "\\", startLine, startColumn, start);

        var sb = new StringBuilder("\\");

        // Read character literal
        while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }

        return new Token(TokenType.Character, sb.ToString(), startLine, startColumn, start);
    }

    private Token ReadKeyword()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;

        // Skip colon
        _position++; 
        _column++;

        var sb = new StringBuilder(":");

        // Keywords can have namespace qualifiers
        while (_position < _input.Length && IsSymbolChar(_input[_position]))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }

        return new Token(TokenType.Keyword, sb.ToString(), startLine, startColumn, start);
    }
    
    private Token ReadDispatch()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;
            
        // Skip #
        _position++; 
        _column++;
            
        if (_position >= _input.Length)
            return new Token(TokenType.Dispatch, "#", startLine, startColumn, start);
            
        char next = _input[_position];
        var sb = new StringBuilder("#");
        
        if (next == '{' || next == '(' || next == '\'' || next == '_' || next == '"' || next == '?')
        {
            sb.Append(next);
            _position++;
            _column++;
        }
            
        return new Token(TokenType.Dispatch, sb.ToString(), startLine, startColumn, start);
    }
    
    private Token ReadNumber()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;
            
        var sb = new StringBuilder();
            
        // Handle negative sign
        if (_input[_position] == '-')
        {
            sb.Append('-');
            _position++;
            _column++;
        }
        
        while (_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == '.' || 
                                             _input[_position] == 'e' || _input[_position] == 'E' || _input[_position] == 'M' || 
                                             _input[_position] == 'N' || _input[_position] == '/' || _input[_position] == 'r'))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }
            
        return new Token(TokenType.Number, sb.ToString(), startLine, startColumn, start);
    }
    
    private Token ReadSymbol()
    {
        int start = _position;
        int startLine = _line;
        int startColumn = _column;
            
        var sb = new StringBuilder();
            
        while (_position < _input.Length && IsSymbolChar(_input[_position]))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }
            
        string value = sb.ToString();
            
        // Check for special values
        if (value == "true" || value == "false")
            return new Token(TokenType.Boolean, value, startLine, startColumn, start);
        if (value == "nil")
            return new Token(TokenType.Nil, value, startLine, startColumn, start);
        if (SpecialForms.Contains(value))
            return new Token(TokenType.SpecialForm, value, startLine, startColumn, start);
            
        return new Token(TokenType.Symbol, value, startLine, startColumn, start);
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
    
    private bool IsSymbolStart(char c)
    {
        return char.IsLetter(c) || "+-*/<>=!?$%&_".Contains(c);
    }

    private bool IsSymbolChar(char c)
    {
        return char.IsLetterOrDigit(c) || "+-*/<>=!?$%&_.:'#".Contains(c);
    }
    
    
}