using Flr.Channel;
using Flr.Impl;

namespace Flr.Tests.Impl.Channel;

public static class ChannelTestExtensions
{
    public static bool Consume(this IChannel<LexerOutput> channel, String code, LexerOutput output)
    {
        return channel.Consume(new CodeReader(code), output);
    }
}
