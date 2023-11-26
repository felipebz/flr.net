using System.Text.RegularExpressions;

using Flr.Api;
using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Impl.Channel;

public partial class CommentRegexChannelTest
{
    [GeneratedRegex("//.*")]
    private static partial Regex Regex();

    private readonly LexerOutput _output = new();
    private readonly CommentRegexChannel _channel = new(Regex());

    [Fact]
    public void TestConsumeOneCharacter()
    {
        Assert.False(_channel.Consume("This is not a comment", _output));
        Assert.True(_channel.Consume("//My Comment\n second line", _output));
        _output.AddToken(Token.Builder()
            .WithType(GenericTokenType.Eof)
            .WithValueAndOriginalValue("EOF")
            .WithLine(1)
            .WithColumn(1)
            .Build());

        var trivia = _output.Tokens.SelectMany(t => t.Trivia).ToList();
        Assert.Contains(trivia, t => t is { IsComment: true, Token.Value: "//My Comment" });
        Assert.Contains(trivia, t => t is { IsComment: true, Token.OriginalValue: "//My Comment" });
    }
}
