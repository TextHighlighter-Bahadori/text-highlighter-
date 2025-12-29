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

    private AstNode ParseExpression()
    {
        throw new NotImplementedException();
    }

    private DefnNode ParseDefn(int startPos, Token defnToken)
        {
            var defnNode = new DefnNode
            {
                DefnKeyword = defnToken,
                StartPosition = startPos,
                IsPrivate = defnToken.Value == "defn-"
            };

            // Skip 'defn'
            Advance(); 
            SkipWhitespaceAndComments();

          
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.Symbol || CurrentToken().Type == TokenType.SpecialForm))
            {
                defnNode.FunctionName = new SymbolNode(CurrentToken(), SymbolRole.FunctionName);
                Advance();
            }

            SkipWhitespaceAndComments();

           
            if (!IsAtEnd() && CurrentToken().Type == TokenType.String)
            {
                defnNode.Docstring = CurrentToken().Value;
                Advance();
                SkipWhitespaceAndComments();
            }
            
            if (!IsAtEnd() && CurrentToken().Type == TokenType.LeftBracket)
            {
                Advance();
                SkipWhitespaceAndComments();

                while (!IsAtEnd() && CurrentToken().Type != TokenType.RightBracket)
                {
                    if (CurrentToken().Type == TokenType.Symbol)
                    {
                        defnNode.Parameters.Add(new SymbolNode(CurrentToken(), SymbolRole.Parameter));
                    }
                    Advance();
                    SkipWhitespaceAndComments();
                }

                if (!IsAtEnd()) Advance();
            }

            SkipWhitespaceAndComments();

            // Parse body
            while (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                var expr = ParseExpression();
                if (expr != null)
                    defnNode.Body.Add(expr);
                SkipWhitespaceAndComments();
            }

            if (!IsAtEnd())
            {
                defnNode.EndPosition = CurrentToken().Position;
                Advance(); // Skip ')'
            }

            return defnNode;
        }

        private DefNode ParseDef(int startPos, Token defToken)
        {
            var defNode = new DefNode
            {
                DefKeyword = defToken,
                StartPosition = startPos
            };

            // Skip 'def'
            Advance(); 
            SkipWhitespaceAndComments();

            // Parse variable name
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.Symbol || CurrentToken().Type == TokenType.SpecialForm))
            {
                defNode.VariableName = new SymbolNode(CurrentToken(), SymbolRole.Variable);
                Advance();
            }

            SkipWhitespaceAndComments();

            // Check for docstring
            if (!IsAtEnd() && CurrentToken().Type == TokenType.String)
            {
                defNode.Docstring = CurrentToken().Value;
                Advance();
                SkipWhitespaceAndComments();
            }

            // Parse value
            if (!IsAtEnd() && CurrentToken().Type != TokenType.RightParen)
            {
                defNode.Value = ParseExpression();
            }

            SkipWhitespaceAndComments();

            if (!IsAtEnd() && CurrentToken().Type == TokenType.RightParen)
            {
                defNode.EndPosition = CurrentToken().Position;
                Advance();
            }

            return defNode;
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