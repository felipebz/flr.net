using System.Text.RegularExpressions;

using Flr.Api;
using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Impl.Channel;

public partial class RegexChannelTest
{
    [GeneratedRegex("[0-9]*")]
    private static partial Regex Regex();

    private readonly LexerOutput _output = new();
    private readonly RegexChannel _channel = new(GenericTokenType.Constant, Regex());

    [Fact]
    public void TestRegexToHandleNumber()
    {
        Assert.False(_channel.Consume("Not a number", _output));
        Assert.True(_channel.Consume("56;", _output));
        Assert.Contains(_output.Tokens, t => ReferenceEquals(t.Type, GenericTokenType.Constant) && t.Value == "56");
    }

    [Fact]
    public void TestColumnNumber()
    {
        Assert.True(_channel.Consume("56;", _output));
        Assert.Equal(0, _output.Tokens.First().Column);
    }
}
