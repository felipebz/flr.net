using Flr.Api;
using Flr.Channel;

namespace Flr.Impl.Channel;

public class UnknownCharacterChannel : IChannel<LexerOutput>
{
    public bool Consume(CodeReader code, LexerOutput output)
    {
        if (code.Peek() == char.MinValue)
        {
            return false;
        }

        var value = code.Pop();
        var token = Token.Builder()
            .WithType(GenericTokenType.UnknownChar)
            .WithValueAndOriginalValue(value.ToString())
            .WithLine(code.LinePosition)
            .WithColumn(code.ColumnPosition - 1)
            .Build();
        output.AddToken(token);
        return true;
    }
}
