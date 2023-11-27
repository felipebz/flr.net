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
    public void TestPeekTo()
    {
        var reader = new CodeReader(new StringReader("package org.sonar;"));
        var result = new StringBuilder();
        reader.PeekTo(endFlag => 'r' == endFlag, result);
        Assert.Equal("package o", result.ToString());
        Assert.Equal('p', reader.Peek()); // never called pop()
    }

    [Fact]
    public void PeekToShouldStopAtEndOfInput()
    {
        var reader = new CodeReader("foo");
        var result = new StringBuilder();
        reader.PeekTo(_ => false, result);
        Assert.Equal("foo", result.ToString());
    }

    [Fact]
    public void TestPopToWithRegex()
    {
        var reader = new CodeReader(new StringReader("123ABC"));
        var token = new StringBuilder();
        Assert.Equal(3, reader.PopTo(DigitsRegex(), token));
        Assert.Equal("123", token.ToString());
        Assert.Equal(-1, reader.PopTo(DigitsRegex(), token));
        Assert.Equal(3, reader.PopTo(CharRegex(), token));
        Assert.Equal("123ABC", token.ToString());
        Assert.Equal(-1, reader.PopTo(CharRegex(), token));
    }

    [Fact]
    public void TestPopToWithRegexAndFollowingMatcher()
    {
        var digitMatcher = new Regex("^\\d+");
        var alphabeticMatcher = new Regex("^[a-zA-Z]+");
        var token = new StringBuilder();
        Assert.Equal(-1, new CodeReader(new StringReader("123 ABC")).PopTo(digitMatcher, alphabeticMatcher, token));
        Assert.Equal("", token.ToString());
        Assert.Equal(3, new CodeReader(new StringReader("123ABCD")).PopTo(digitMatcher, alphabeticMatcher, token));
        Assert.Equal("123", token.ToString());
    }

    [GeneratedRegex("\\d+")]
    private static partial Regex DigitsRegex();

    [GeneratedRegex("\\w+")]
    private static partial Regex CharRegex();
}
