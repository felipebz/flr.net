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
        var span = code.PopTo(regex);
        if (span.Length == 0)
        {
            return false;
        }

        Debug.Assert(code.PreviousCursor != null, "code.PreviousCursor != null");

        var token = Token.Builder()
            .WithType(type)
            .WithValueAndOriginalValue(span.ToString())
            .WithLine(code.PreviousCursor.Line)
            .WithColumn(code.PreviousCursor.Column)
            .Build();
        output.AddToken(token);
        return true;
    }
}
