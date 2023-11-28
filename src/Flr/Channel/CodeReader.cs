using System.Text;
using System.Text.RegularExpressions;

namespace Flr.Channel;

public class CodeReader : CodeBuffer
{
    public CodeReader(TextReader initialCodeReader, CodeReaderConfiguration configuration) : base(initialCodeReader,
        configuration)
    {
    }

    public CodeReader(string code, CodeReaderConfiguration configuration) : base(code, configuration)
    {
    }

    public CodeReader(TextReader initialCodeReader) : base(initialCodeReader, new CodeReaderConfiguration())
    {
    }

    public CodeReader(string code) : base(code, new CodeReaderConfiguration())
    {
    }

    public Cursor? PreviousCursor { get; private set; }

    public void Pop(StringBuilder builder)
    {
        builder.Append(Pop());
    }

    public char[] Peek(int length)
    {
        var result = new char[length];
        var index = 0;
        var nextChar = CharAt(index);
        while (nextChar != char.MinValue && index < length)
        {
            result[index++] = nextChar;
            nextChar = CharAt(index);
        }

        return result;
    }

    public void PeekTo(Func<char, bool> matcher, StringBuilder builder)
    {
        var index = 0;
        var nextChar = CharAt(index);
        while (nextChar != char.MinValue && !matcher(nextChar))
        {
            builder.Append(nextChar);
            nextChar = CharAt(++index);
        }
    }

    public ReadOnlySpan<char> PopTo(Regex matcher, Regex? afterMatcher = null)
    {
        var match = matcher.Match(Buffer, BufferPosition, Buffer.Length - BufferPosition);
        if (match.Success && afterMatcher != null)
        {
            var afterMatch = afterMatcher.Match(Buffer, (BufferPosition + match.Length),
                Buffer.Length - (BufferPosition + match.Length));
            if (!afterMatch.Success)
            {
                return ReadOnlySpan<char>.Empty;
            }
        }

        if (match.Success)
        {
            PreviousCursor = _cursor.Clone();
            Pop(match.ValueSpan.Length);

            return match.ValueSpan;
        }

        return ReadOnlySpan<char>.Empty;
    }
}
