namespace Flr.Api;

public interface ITokenType : IAstNodeType
{
    public string Name { get; }
    public string Value { get; }
    public bool HasToBeSkippedFromAst(AstNode? node);
}
