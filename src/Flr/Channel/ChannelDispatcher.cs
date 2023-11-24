﻿namespace Flr.Channel;

public class ChannelDispatcher<T> : IChannel<T>
{
    private readonly bool failIfNoChannelToConsumeOneCharacter;
    public List<IChannel<T>> Channels { get; }

    private ChannelDispatcher(ChannelDispatcherBuilder builder)
    {
        failIfNoChannelToConsumeOneCharacter = builder.FailIfNoChannelToConsumeOneCharacterOption;
        Channels = builder.Channels;
    }

    public bool Consume(CodeReader code, T output)
    {
        var nextChar = code.Peek();
        while (nextChar != char.MinValue)
        {
            var characterConsumed = Channels.Any(channel => channel.Consume(code, output));
            if (!characterConsumed)
            {
                if (failIfNoChannelToConsumeOneCharacter)
                {
                    var message = $"None of the channel has been able to handle character '{code.Peek()}' (decimal value {code.Peek()}) at line {code.LinePosition}, column {code.ColumnPosition}";
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