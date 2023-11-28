namespace Flr.Channel;

public class ChannelDispatcher<T> : IChannel<T>
{
    private readonly bool _failIfNoChannelToConsumeOneCharacter;
    public IChannel<T>[] Channels { get; }

    private ChannelDispatcher(ChannelDispatcherBuilder builder)
    {
        _failIfNoChannelToConsumeOneCharacter = builder.FailIfNoChannelToConsumeOneCharacterOption;
        Channels = builder.Channels.ToArray();
    }

    public bool Consume(CodeReader code, T output)
    {
        var nextChar = code.Peek();
        while (nextChar != char.MinValue)
        {
            var characterConsumed = false;
            foreach (var channel in Channels)
            {
                if (channel.Consume(code, output))
                {
                    characterConsumed = true;
                    break;
                }
            }

            if (!characterConsumed)
            {
                if (_failIfNoChannelToConsumeOneCharacter)
                {
                    var message =
                        $"None of the channel has been able to handle character '{code.Peek()}' (decimal value {(int)code.Peek()}) at line {code.LinePosition}, column {code.ColumnPosition}";
                    throw new InvalidOperationException(message);
                }

                code.Pop();
            }

            nextChar = code.Peek();
        }

        return true;
    }

    public class ChannelDispatcherBuilder
    {
        public List<IChannel<T>> Channels { get; } = new();
        public bool FailIfNoChannelToConsumeOneCharacterOption { get; private set; }

        public ChannelDispatcherBuilder AddChannel(IChannel<T> channel)
        {
            Channels.Add(channel);
            return this;
        }

        public ChannelDispatcherBuilder AddChannels(params IChannel<T>[] channels)
        {
            foreach (var channel in channels)
            {
                AddChannel(channel);
            }

            return this;
        }

        public ChannelDispatcherBuilder AddChannels(IEnumerable<IChannel<T>> channels)
        {
            foreach (var channel in channels)
            {
                AddChannel(channel);
            }

            return this;
        }

        public ChannelDispatcherBuilder FailIfNoChannelToConsumeOneCharacter()
        {
            FailIfNoChannelToConsumeOneCharacterOption = true;
            return this;
        }

        public ChannelDispatcher<T> Build()
        {
            return new ChannelDispatcher<T>(this);
        }
    }

    public static ChannelDispatcherBuilder Builder()
    {
        return new ChannelDispatcherBuilder();
    }
}
