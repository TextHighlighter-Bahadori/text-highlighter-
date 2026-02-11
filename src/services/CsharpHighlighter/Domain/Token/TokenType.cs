namespace CsharpHighlighter.Domain.Token;

public enum TokenType
{
    Keyword,
    
    Identifier,
    
    String,
    Number,
    Character,
    Boolean,
    Null,
    
    Operator,
    
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Semicolon,
    Comma,
    Dot,
    Colon,
    
    Whitespace,
    Comment,
    Preprocessor,
    Attribute,
    
    Unknown,
    EOF
}