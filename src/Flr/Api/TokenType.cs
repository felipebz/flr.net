namespace Flr.Api;

public interface TokenType : AstNodeType
{
    public string Name { get; }
    public string Value { get; }
    public bool HasToBeSkippedFromAst(AstNode? node);
}