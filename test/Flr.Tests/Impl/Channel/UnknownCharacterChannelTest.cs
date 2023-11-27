using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Impl.Channel;

public class UnknownCharacterChannelTest
{
    private readonly LexerOutput _output = new();
    private readonly UnknownCharacterChannel _channel = new();

    [Fact]
    public void TestConsumeOneCharacter()
    {
        Assert.True(_channel.Consume("'", _output));
        Assert.True(_channel.Consume("a", _output));

        Assert.Equal(2, _output.Tokens.Count());
        Assert.Collection(_output.Tokens,
            t => Assert.Equal("'", t.Value),
            t => Assert.Equal("a", t.Value));
    }
}
