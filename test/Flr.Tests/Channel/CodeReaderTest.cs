using System.Text;
using System.Text.RegularExpressions;

using Flr.Channel;

namespace Flr.Tests.Channel;

public partial class CodeReaderTest
{
    [Fact]
    public void TestPopWithAppendable()
    {
        var reader = new CodeReader("package org.sonar;");
        var sw = new StringBuilder();
        reader.Pop(sw);
        Assert.Equal("p", sw.ToString());
        reader.Pop(sw);
        Assert.Equal("pa", sw.ToString());
    }

    [Fact]
    public void TestPeekACharArray()
    {
        var reader = new CodeReader("bar");
        var chars = reader.Peek(2);
        Assert.Equal(2, chars.Length);
        Assert.Equal('b', chars[0]);
        Assert.Equal('a', chars[1]);
    }

    [Fact]
    public void TestPopToWithRegex()
    {
        var reader = new CodeReader(new StringReader("123ABC"));

        var span = reader.PopTo(DigitsRegex());
        Assert.Equal("123", span.ToString());

        span = reader.PopTo(CharRegex());
        Assert.Equal("ABC", span.ToString());
    }

    [Fact]
    public void TestPopToWithRegexAndFollowingMatcher()
    {
        var digitMatcher = new Regex("^\\d+");
        var alphabeticMatcher = new Regex("^[a-zA-Z]+");

        var span = new CodeReader(new StringReader("123 ABC")).PopTo(digitMatcher, alphabeticMatcher);
        Assert.Equal("", span);

        span = new CodeReader(new StringReader("123ABCD")).PopTo(digitMatcher, alphabeticMatcher);
        Assert.Equal("123", span.ToString());
    }

    [GeneratedRegex("\\d+")]
    private static partial Regex DigitsRegex();

    [GeneratedRegex("\\w+")]
    private static partial Regex CharRegex();
}
