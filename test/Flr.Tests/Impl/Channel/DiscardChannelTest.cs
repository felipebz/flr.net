using System.Text.RegularExpressions;

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
        Assert.True(_channel.Consume(" ", _output));
        Assert.True(_channel.Consume("\t", _output));
        Assert.False(_channel.Consume("g", _output));
        Assert.False(_channel.Consume("-", _output));
        Assert.False(_channel.Consume("1", _output));
    }
}
