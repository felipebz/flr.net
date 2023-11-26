namespace Flr.Api;

public class GenericTokenType : ITokenType
{
    public static GenericTokenType Comment = new("COMMENT");
    public static GenericTokenType Identifier = new("IDENTIFIER");
    public static GenericTokenType Literal = new("LITERAL");
    public static GenericTokenType Constant = new("CONSTANT");
    public static GenericTokenType Eof = new("EOF");
    public static GenericTokenType Eol = new("EOL");
    public static GenericTokenType UnknownChar = new("UNKNOWN_CHAR");

    private GenericTokenType(string identifier)
    {
        Name = identifier;
        Value = identifier;
    }

    public string Name { get; }
    public string Value { get; }

    public bool HasToBeSkippedFromAst(AstNode? node) => false;
}
