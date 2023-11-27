namespace Flr.Api;

public record GenericTokenType : ITokenType
{
    public readonly static GenericTokenType Comment = new(nameof(Comment));
    public readonly static GenericTokenType Identifier = new(nameof(Identifier));
    public readonly static GenericTokenType Literal = new(nameof(Literal));
    public readonly static GenericTokenType Constant = new(nameof(Constant));
    public readonly static GenericTokenType Eof = new(nameof(Eof));
    public readonly static GenericTokenType Eol = new(nameof(Eol));
    public readonly static GenericTokenType UnknownChar = new(nameof(UnknownChar));

    public readonly static IEnumerable<ITokenType> Values = new[]
    {
        Comment, Identifier, Literal, Constant, Eof, Eol, UnknownChar
    };

    private GenericTokenType(string identifier)
    {
        Name = identifier;
        Value = identifier;
    }

    public string Name { get; }
    public string Value { get; }

    public bool HasToBeSkippedFromAst(AstNode? node) => false;
}
