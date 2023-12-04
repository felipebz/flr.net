using Flr.Api;
using Flr.Channel;

namespace Flr.Impl.Channel;

public class PunctuatorChannel : IChannel<LexerOutput>
{
    private readonly (ITokenType Type, string Value)[] _sortedPunctuators;
    private readonly int _lookahead;

    public PunctuatorChannel(IEnumerable<ITokenType> punctuators)
    {
        _sortedPunctuators = punctuators
            .OrderByDescending(x => x.Value.Length)
            .Select(x => (x, x.Value))
            .ToArray();
        _lookahead = _sortedPunctuators.First().Value.Length;
    }

    public bool Consume(CodeReader code, LexerOutput output)
    {
        var next = code.Peek(_lookahead);
        foreach (var punctuator in _sortedPunctuators)
        {
            if (!ArraysEquals(punctuator.Value.AsSpan(), next))
            {
                continue;
            }

            var token = Token.Builder()
                .WithType(punctuator.Type)
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

    private static bool ArraysEquals(ReadOnlySpan<char> punctuatorChars, ReadOnlySpan<char> next)
    {
        if (punctuatorChars.Length > next.Length) return false;

        for (var i = 0; i < punctuatorChars.Length; i++)
        {
            if (punctuatorChars[i] != next[i])
            {
                return false;
            }
        }

        return true;
    }
}
