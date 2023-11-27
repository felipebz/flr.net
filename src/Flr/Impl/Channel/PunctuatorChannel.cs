using Flr.Api;
using Flr.Channel;

namespace Flr.Impl.Channel;

public class PunctuatorChannel : IChannel<LexerOutput>
{
    private readonly ITokenType[] _sortedPunctuators;
    private readonly int _lookahead;

    public PunctuatorChannel(IEnumerable<ITokenType> punctuators)
    {
        _sortedPunctuators = punctuators.OrderByDescending(x => x.Value.Length).ToArray();
        _lookahead = _sortedPunctuators.First().Value.Length;
    }

    public bool Consume(CodeReader code, LexerOutput output)
    {
        var next = code.Peek(_lookahead);
        foreach (var punctuator in _sortedPunctuators)
        {
            if (!next[..punctuator.Value.Length].SequenceEqual(punctuator.Value.ToCharArray()))
            {
                continue;
            }

            var token = Token.Builder()
                .WithType(punctuator)
                .WithValueAndOriginalValue(punctuator.Value)
                .WithLine(code.LinePosition)
                .WithColumn(code.ColumnPosition)
                .Build();
            output.AddToken(token);
            code.Pop(punctuator.Value.Length);
            return true;
        }

        return false;
    }
}
