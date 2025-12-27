namespace ClojureHighlighter.Domain;

public enum TokenType
{
    LeftParen,
    RightParen,
    LeftBracket,
    RightBracket,
    LeftBrace,
    RightBrace,
    String,
    Number,
    Character,
    Keyword,
    Boolean,
    Nil,
    Symbol,
    SpecialForm,
    Comment,
    Quote,
    Deref,
    Metadata,
    Dispatch,
    Unquote,
    UnquoteSplicing,
    SyntaxQuote,
    Whitespace,
    EOF
}