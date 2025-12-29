using ClojureHighlighter.Domain;
using ClojureHighlighter.Domain.ASTNode;

namespace ClojureHighlighter.Application;

public class ClojureParserService : IClojureParserService
{
    private List<Token> _tokens;
    private int _current;
    private readonly HashSet<string> _definitionForms = new HashSet<string>
    {
        "def", "defn", "defn-", "defmacro", "defmethod", "defmulti",
        "defprotocol", "defrecord", "defstruct", "deftype"
    };

    private readonly HashSet<string> _bindingForms = new HashSet<string>
    {
        "let", "letfn", "binding", "loop", "for", "doseq"
    };

    private readonly HashSet<string> _conditionalForms = new HashSet<string>
    {
        "if", "if-not", "if-let", "when", "when-not", "when-let", "cond", "condp", "case"
    };

    public ClojureParserService(List<Token> tokens)
    {
        _tokens = tokens;
        _current = 0;
    }
    
    
    public List<AstNode> Parse()
    {
        throw new NotImplementedException();
    }

    
    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd() && 
               (CurrentToken().Type == TokenType.Whitespace || 
                CurrentToken().Type == TokenType.Comment))
        {
            Advance();
        }
    }

    private Token CurrentToken()
    {
        return _tokens[_current];
    }

    private Token PeekToken(int offset = 1)
    {
        int index = _current + offset;
        if (index < _tokens.Count)
            return _tokens[index];
        return _tokens[_tokens.Count - 1];
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