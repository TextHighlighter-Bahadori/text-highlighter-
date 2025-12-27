using ClojureHighlighter.Domain;

namespace ClojureHighlighter.Application;

public class ClojureTokenizer
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
    
    
    public ClojureTokenizer(string input)
    {
        _input = input;
        _position = 0;
        _line = 1;
        _column = 1;
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

}