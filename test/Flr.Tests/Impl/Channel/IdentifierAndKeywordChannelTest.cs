using System.Text.RegularExpressions;

using Flr.Api;
using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Impl.Channel;

public partial class IdentifierAndKeywordChannelTest
{
    [GeneratedRegex("[a-zA-Z_][a-zA-Z_0-9]*")]
    private static partial Regex Regex();

    private readonly LexerOutput _output = new();
    private IdentifierAndKeywordChannel _channel = null!;

    [Fact]
    public void TestConsumeWord()
    {
        _channel = new IdentifierAndKeywordChannel(Regex(), true, MyKeywords.Values);
        Assert.True(_channel.Consume("word", _output));
        Assert.Equal("word", _output.Tokens[0].Value);
        Assert.Same(GenericTokenType.Identifier, _output.Tokens[0].Type);
    }

    [Fact]
    public void TestConsumeCaseSensitiveKeywords()
    {
        _channel = new IdentifierAndKeywordChannel(Regex(), true, MyKeywords.Values);

        Assert.True(_channel.Consume("KEYWORD1", _output));
        Assert.Equal("KEYWORD1", _output.Tokens[0].Value);
        Assert.Same(MyKeywords.Keyword1, _output.Tokens[0].Type);

        Assert.True(_channel.Consume("KeyWord2", _output));
        Assert.Equal("KeyWord2", _output.Tokens[1].Value);
        Assert.Same(MyKeywords.Keyword2, _output.Tokens[1].Type);

        Assert.True(_channel.Consume("KEYWORD2", _output));
        Assert.Equal("KEYWORD2", _output.Tokens[2].Value);
        Assert.Same(GenericTokenType.Identifier, _output.Tokens[2].Type);
    }

    [Fact]
    public void TestConsumeNotCaseSensitiveKeywords()
    {
        _channel = new IdentifierAndKeywordChannel(Regex(), false, MyKeywords.Values);

        Assert.True(_channel.Consume("keyword1", _output));
        Assert.Equal("KEYWORD1", _output.Tokens[0].Value);
        Assert.Equal("keyword1", _output.Tokens[0].OriginalValue);
        Assert.Same(MyKeywords.Keyword1, _output.Tokens[0].Type);

        Assert.True(_channel.Consume("keyword2", _output));
        Assert.Equal("KEYWORD2", _output.Tokens[1].Value);
        Assert.Equal("keyword2", _output.Tokens[1].OriginalValue);
        Assert.Same(MyKeywords.Keyword2, _output.Tokens[1].Type);
    }

    [Fact]
    public void TestNotConsumeNumber()
    {
        _channel = new IdentifierAndKeywordChannel(Regex(), true, MyKeywords.Values);
        Assert.False(_channel.Consume("1234", _output));
    }

    record MyKeywords : ITokenType
    {
        public readonly static MyKeywords Keyword1 = new(nameof(Keyword1), "KEYWORD1");
        public readonly static MyKeywords Keyword2 = new(nameof(Keyword2), "KeyWord2");

        public readonly static IEnumerable<ITokenType> Values = new[] { Keyword1, Keyword2 };

        private MyKeywords(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public bool HasToBeSkippedFromAst(AstNode? node)
        {
            return false;
        }
    }
}
