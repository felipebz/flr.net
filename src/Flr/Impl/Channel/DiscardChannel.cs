using System.Text.RegularExpressions;

using Flr.Channel;

namespace Flr.Impl.Channel;

public class DiscardChannel(Regex regex) : IChannel<LexerOutput>
{
    public bool Consume(CodeReader code, LexerOutput output)
    {
        return code.PopTo(regex, null) != -1;
    }
}
