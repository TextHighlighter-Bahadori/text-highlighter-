namespace CsharpHighlighter.Domain.Token;

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public int Position { get; set; }

    public Token(TokenType type, string value, int line, int column, int position)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
        Position = position;
    }
}
