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

    public string PopTo(Regex matcher, Regex? afterMatcher = null)
    {
        var enumerator = matcher.EnumerateMatches(Buffer.AsSpan(BufferPosition));
        var success = enumerator.MoveNext();
        var match = enumerator.Current;
        if (success)
        {
            if (afterMatcher != null)
            {
                {
                    if (!afterMatcher.IsMatch(Buffer.AsSpan(BufferPosition + match.Length)))
                    {
                        return string.Empty;
                    }
                }
            }

            PreviousCursor = _cursor.Clone();
            var valor = Buffer.Substring(BufferPosition, match.Length);
            Pop(match.Length);

            return valor;
        }

        return string.Empty;
        return string.Empty;
    }
}
