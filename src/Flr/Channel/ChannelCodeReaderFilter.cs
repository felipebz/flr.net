namespace Flr.Channel;

public class ChannelCodeReaderFilter<T>(params IChannel<T>[] channels) : CodeReaderFilter<T>
{
    private CodeReader _internalCodeReader = null!;

    public IEnumerable<IChannel<T>> Channels { get; } = channels;

    public override TextReader Reader
    {
        get => base.Reader;
        set
        {
            base.Reader = value;
            _internalCodeReader = new CodeReader(value, Configuration);
        }
    }

    public override int Read(char[] filteredBuffer, int offset, int length)
    {
        var currentOffset = offset;
        if (_internalCodeReader.Peek() == char.MinValue)
        {
            return 0;
        }

        var initialOffset = currentOffset;
        while (currentOffset < filteredBuffer.Length) {
            if (_internalCodeReader.Peek() == char.MinValue)
            {
                break;
            }

            var consumed = Channels.Any(channel => channel.Consume(_internalCodeReader, Output));
            if (!consumed)
            {
                filteredBuffer[currentOffset++] = _internalCodeReader.Pop();
            }
        }

        return currentOffset - initialOffset;
    }
}