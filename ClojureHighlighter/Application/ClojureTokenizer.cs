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

}