using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using Flr.Api;
using Flr.Channel;

namespace Flr.Impl.Channel;

public class RegexChannel(ITokenType type, Regex regex) : IChannel<LexerOutput>
{
    public bool Consume(CodeReader code, LexerOutput output)
    {
        var builder = new StringBuilder();
        if (code.PopTo(regex, builder) <= 0)
        {
            return false;
        }

        Debug.Assert(code.PreviousCursor != null, "code.PreviousCursor != null");

        var value = builder.ToString();
        var token = Token.Builder()
            .WithType(type)
            .WithValueAndOriginalValue(value)
            .WithLine(code.PreviousCursor.Line)
            .WithColumn(code.PreviousCursor.Column)
            .Build();
        output.AddToken(token);
        builder.Clear();
        return true;
    }
}
