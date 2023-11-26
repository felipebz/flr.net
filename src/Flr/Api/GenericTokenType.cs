namespace Flr.Api;

public class GenericTokenType : TokenType
{
    public static GenericTokenType COMMENT = new("COMMENT");
    public static GenericTokenType IDENTIFIER = new("IDENTIFIER");
    public static GenericTokenType LITERAL = new("LITERAL");
    public static GenericTokenType CONSTANT = new("CONSTANT");
    public static GenericTokenType EOF = new("EOF");
    public static GenericTokenType EOL = new("EOL");
    public static GenericTokenType UNKNOWN_CHAR = new("UNKNOWN_CHAR");
    
    private GenericTokenType(string identifier)
    {
        Name = identifier;
        Value = identifier;
    }
    
    public string Name { get; }
    public string Value { get; }

    public bool HasToBeSkippedFromAst(AstNode? node) => false;
}