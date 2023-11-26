using System.Text.RegularExpressions;

using Flr.Channel;

namespace Flr.Tests.Channel;

public partial class CodeBufferTest
{
    private readonly CodeReaderConfiguration _configuration = new();

    [Fact]
    public void TestPop()
    {
        var code = new CodeBuffer("pa", _configuration);
        Assert.Equal('p', code.Pop());
        Assert.Equal('a', code.Pop());
        Assert.Equal(char.MinValue, code.Pop());
    }

    [Fact]
    public void TestPeek()
    {
        var code = new CodeBuffer("pa", _configuration);
        Assert.Equal('p', code.Peek());
        Assert.Equal('p', code.Peek());
        code.Pop();
        Assert.Equal('a', code.Peek());
        code.Pop();
        Assert.Equal(char.MinValue, code.Pop());
    }

    [Fact]
    public void TestLastCharacter()
    {
        var code = new CodeBuffer("bar", _configuration);
        Assert.Equal(char.MinValue, code.LastChar);
        code.Pop();
        Assert.Equal('b', code.LastChar);
    }

    [Fact]
    public void TestGetColumnAndLinePosition()
    {
        var code = new CodeBuffer("pa\nc\r\ns\r\n\r\n", _configuration);
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(1, code.LinePosition);
        code.Pop(); // p
        code.Pop(); // a
        Assert.Equal(2, code.ColumnPosition);
        Assert.Equal(1, code.LinePosition);
        code.Peek(); // \n
        Assert.Equal(2, code.ColumnPosition);
        Assert.Equal(1, code.LinePosition);
        code.Pop(); // \n
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(2, code.LinePosition);
        code.Pop(); // c
        Assert.Equal(1, code.ColumnPosition);
        Assert.Equal(2, code.LinePosition);
        code.Pop(); // \r
        code.Pop(); // \n
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(3, code.LinePosition);
        Assert.Equal('s', code.Pop());
        code.Pop(); // \r
        Assert.Equal(2, code.ColumnPosition);
        Assert.Equal(3, code.LinePosition);
        code.Pop(); // \n
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(4, code.LinePosition);
        code.Pop(); // \r
        code.Pop(); // \n
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(5, code.LinePosition);
    }

    [Fact]
    public void TestCharAt()
    {
        var code = new CodeBuffer("123456", _configuration);
        Assert.Equal('1', code[0]);
        Assert.Equal('6', code[5]);
    }

    [Fact]
    public void TestCharAtIndexOutOfBoundsException()
    {
        var code = new CodeBuffer("12345", _configuration);
        Assert.Equal(char.MinValue, code[5]);
    }

    [Fact]
    public void TestReadWithSpecificTabWidth()
    {
        var code = new CodeBuffer("pa\n\tc", new CodeReaderConfiguration { TabWidth = 4 });
        Assert.Equal('\n', code[2]);
        Assert.Equal('\t', code[3]);
        Assert.Equal('c', code[4]);
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(1, code.LinePosition);
        code.Pop(); // p
        code.Pop(); // a
        Assert.Equal(2, code.ColumnPosition);
        Assert.Equal(1, code.LinePosition);
        code.Peek(); // \n
        Assert.Equal(2, code.ColumnPosition);
        Assert.Equal(1, code.LinePosition);
        code.Pop(); // \n
        Assert.Equal(0, code.ColumnPosition);
        Assert.Equal(2, code.LinePosition);
        code.Pop(); // \t
        Assert.Equal(4, code.ColumnPosition);
        Assert.Equal(2, code.LinePosition);
        code.Pop(); // c
        Assert.Equal(5, code.ColumnPosition);
        Assert.Equal(2, code.LinePosition);
    }

    [Fact]
    public void TestCodeReaderFilter()
    {
        var code = new CodeBuffer("abcd12efgh34",
            new CodeReaderConfiguration { Filters = { new ReplaceNumbersFilter() } });
        Assert.Equal('a', code[0]);
        Assert.Equal('-', code[4]);
        Assert.Equal('-', code[5]);
        Assert.Equal('e', code[6]);
        Assert.Equal('-', code[10]);
        Assert.Equal('-', code[11]);
        Assert.Equal('a', code.Peek());
        Assert.Equal('a', code.Pop());
        Assert.Equal('b', code.Pop());
        Assert.Equal('c', code.Pop());
        Assert.Equal('d', code.Pop());
        Assert.Equal('-', code.Peek());
        Assert.Equal('-', code.Pop());
        Assert.Equal('-', code.Pop());
        Assert.Equal('e', code.Pop());
        Assert.Equal('f', code.Pop());
        Assert.Equal('g', code.Pop());
        Assert.Equal('h', code.Pop());
        Assert.Equal('-', code.Pop());
        Assert.Equal('-', code.Pop());
    }

    [Fact]
    public void TheLengthShouldBeTheSameThanTheStringLength()
    {
        var code = new CodeBuffer("myCode", _configuration);
        Assert.Equal(6, code.Length);
    }

    [Fact]
    public void TheLengthShouldDecreaseEachTimeTheInputStreamIsConsumed()
    {
        var code = new CodeBuffer("myCode", _configuration);
        code.Pop();
        code.Pop();
        Assert.Equal(4, code.Length);
    }

    [Fact]
    public void TestSeveralCodeReaderFilter()
    {
        var code = new CodeBuffer("abcd12efgh34",
            new CodeReaderConfiguration { Filters = { new ReplaceNumbersFilter(), new ReplaceCharFilter() } });
        Assert.Equal('*', code[0]);
        Assert.Equal('-', code[4]);
        Assert.Equal('-', code[5]);
        Assert.Equal('*', code[6]);
        Assert.Equal('-', code[10]);
        Assert.Equal('-', code[11]);
        Assert.Equal('*', code.Peek());
        Assert.Equal('*', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('-', code.Peek());
        Assert.Equal('-', code.Pop());
        Assert.Equal('-', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('*', code.Pop());
        Assert.Equal('-', code.Pop());
        Assert.Equal('-', code.Pop());
    }

    [Fact]
    public void TestChannelCodeReaderFilter()
    {
        var code = new CodeBuffer("0123456789\nABCDEFGHIJ",
            new CodeReaderConfiguration { Filters = { new ChannelCodeReaderFilter<object>(new WindowingChannel()) } });
        Assert.Equal('2', code[0]);
        Assert.Equal('7', code[5]);
        Assert.Equal('\n', code[6]);
        Assert.Equal('C', code[7]);
        Assert.Equal('H', code[12]);
        Assert.Equal(char.MinValue, code.CharAt(13));
        Assert.Equal('2', code.Peek());
        Assert.Equal('2', code.Pop());
        Assert.Equal('3', code.Pop());
        Assert.Equal('4', code.Pop());
        Assert.Equal('5', code.Pop());
        Assert.Equal('6', code.Pop());
        Assert.Equal('7', code.Pop()); // and 8 shouldn't show up
        Assert.Equal('\n', code.Pop());
        Assert.Equal('C', code.Peek());
        Assert.Equal('C', code.Pop());
        Assert.Equal('D', code.Pop());
        Assert.Equal('E', code.Pop());
        Assert.Equal('F', code.Pop());
        Assert.Equal('G', code.Pop());
        Assert.Equal('H', code.Pop());
        Assert.Equal(char.MinValue, code.Pop());
    }

    private partial class ReplaceCharFilter : CodeReaderFilter<object>
    {
        [GeneratedRegex("[a-zA-Z]")]
        private static partial Regex CharRegex();

        public override int Read(char[] filteredBuffer, int offset, int length)
        {
            var tempBuffer = new char[filteredBuffer.Length];
            var charCount = Reader.Read(tempBuffer, offset, length);
            if (charCount != 0)
            {
                var filteredString = CharRegex().Replace(new string(tempBuffer), "*");
                filteredString.CopyTo(0, filteredBuffer, offset, filteredString.Length);
            }

            return charCount;
        }
    }

    private partial class ReplaceNumbersFilter : CodeReaderFilter<object>
    {
        [GeneratedRegex("[0-9]")]
        private static partial Regex DigitsRegex();

        public override int Read(char[] filteredBuffer, int offset, int length)
        {
            var tempBuffer = new char[filteredBuffer.Length];
            var charCount = Reader.Read(tempBuffer, offset, length);
            if (charCount != 0)
            {
                var filteredString = DigitsRegex().Replace(new string(tempBuffer), "-");
                filteredString.CopyTo(0, filteredBuffer, offset, filteredString.Length);
            }

            return charCount;
        }
    }

    private class WindowingChannel : IChannel<object>
    {
        public bool Consume(CodeReader code, object output)
        {
            var columnPosition = code.ColumnPosition;
            if (code.Peek() == '\n')
            {
                return false;
            }

            if (columnPosition is < 2 or > 7)
            {
                code.Pop();
                return true;
            }

            return false;
        }
    }
}
