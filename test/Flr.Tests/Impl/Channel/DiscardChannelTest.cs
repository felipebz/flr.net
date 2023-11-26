using System.Text.RegularExpressions;

using Flr.Channel;
using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Impl.Channel;

public partial class DiscardChannelTest
{
    [GeneratedRegex("[ \\t]+")]
    private static partial Regex Regex();

    private readonly LexerOutput _output = new();
    private readonly DiscardChannel _channel = new(Regex());

    [Fact]
    public void TestConsumeOneCharacter()
    {
        Assert.True(Consume(" ", _output));
        Assert.True(Consume("\t", _output));
        Assert.False(Consume("g", _output));
        Assert.False(Consume("-", _output));
        Assert.False(Consume("1", _output));
    }

    private bool Consume(String code, LexerOutput output)
    {
        return _channel.Consume(new CodeReader(code), output);
    }
}
