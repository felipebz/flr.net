using System.Text;
using System.Text.RegularExpressions;
using Flr.Channel;

namespace Flr.Tests.Channel;

public partial class RegexChannelTest
{
    [Fact]
    public void ShouldMatch()
    {
        var dispatcher = ChannelDispatcher<StringBuilder>.Builder()
            .AddChannels(new MyWordChannel(), new BlackholeChannel())
            .Build();
        var output = new StringBuilder();
        dispatcher.Consume(new CodeReader("my word"), output);
        Assert.Equal("<w>my</w> <w>word</w>", output.ToString());
    }
    
    [Fact]
    public void ShouldMatchTokenLongerThanBuffer()
    {
        var dispatcher = ChannelDispatcher<StringBuilder>.Builder()
            .AddChannel(new MyLiteralChannel())
            .Build();
        var output = new StringBuilder();
        var veryLongLiteral = new string('a', 10000);
        dispatcher.Consume(new CodeReader($"\">{veryLongLiteral}<\""), output);
        Assert.Equal($"<literal>\">{veryLongLiteral}<\"</literal>", output.ToString());
    }

    public partial class MyLiteralChannel() : RegexChannel<StringBuilder>(Regex())
    {
        [GeneratedRegex("^\"[^\"]*\"")]
        private static partial Regex Regex();
        
        protected override void Consume(StringBuilder builder, StringBuilder output)
        {
            output.Append($"<literal>{builder}</literal>");
        }
    }

    private partial class MyWordChannel() : RegexChannel<StringBuilder>(Regex())
    {
        [GeneratedRegex("^\\w+")]
        private static partial Regex Regex();
        
        protected override void Consume(StringBuilder builder, StringBuilder output)
        {
            output.Append($"<w>{builder}</w>");
        }
    }

    private class BlackholeChannel: IChannel<StringBuilder>
    {
        public bool Consume(CodeReader code, StringBuilder output)
        {
            output.Append(code.Pop());
            return true;
        }
    }
}