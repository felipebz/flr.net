using System.Text;

using Flr.Channel;

namespace Flr.Tests.Channel;

public class ChannelDispatcherTest
{
    [Fact]
    public void ShouldRemoveSpacesFromString()
    {
        var dispatcher = ChannelDispatcher<StringBuilder>.Builder().AddChannel(new SpaceDeletionChannel()).Build();
        var output = new StringBuilder();
        dispatcher.Consume(new CodeReader("two words"), output);
        Assert.Equal("twowords", output.ToString());
    }

    [Fact]
    public void ShouldAddChannels()
    {
        var dispatcher = ChannelDispatcher<StringBuilder>.Builder()
            .AddChannels(new SpaceDeletionChannel(), new FakeChannel()).Build();
        Assert.Equal(2, dispatcher.Channels.Count);
        Assert.IsType<SpaceDeletionChannel>(dispatcher.Channels[0]);
        Assert.IsType<FakeChannel>(dispatcher.Channels[1]);
    }

    [Fact]
    public void ShouldThrowExceptionWhenNoChannelToConsumeNextCharacter()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var dispatcher = ChannelDispatcher<StringBuilder>.Builder().FailIfNoChannelToConsumeOneCharacter().Build();
            dispatcher.Consume(new CodeReader("two words"), new StringBuilder());
        });
    }

    private class SpaceDeletionChannel : IChannel<StringBuilder>
    {
        public bool Consume(CodeReader code, StringBuilder output)
        {
            if (code.Peek() == ' ')
            {
                code.Pop();
            }
            else
            {
                output.Append(code.Pop());
            }

            return true;
        }
    }

    private class FakeChannel : IChannel<StringBuilder>
    {
        public bool Consume(CodeReader code, StringBuilder output)
        {
            return true;
        }
    }
}
