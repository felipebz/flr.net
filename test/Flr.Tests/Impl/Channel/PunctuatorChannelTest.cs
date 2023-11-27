using Flr.Api;
using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Impl.Channel;

public class PunctuatorChannelTest
{
    private readonly LexerOutput _output = new();
    private readonly PunctuatorChannel _channel = new(MyPunctuatorAndOperator.Values);

    [Fact]
    public void TestConsumeSpecialCharacters()
    {
        Assert.True(_channel.Consume("**=", _output));
        Assert.Equal("*", _output.Tokens[0].Value);
        Assert.Same(MyPunctuatorAndOperator.Star, _output.Tokens[0].Type);

        Assert.True(_channel.Consume(",=", _output));
        Assert.Equal(",", _output.Tokens[1].Value);
        Assert.Same(MyPunctuatorAndOperator.Colon, _output.Tokens[1].Type);

        Assert.True(_channel.Consume("=*", _output));
        Assert.Equal("=", _output.Tokens[2].Value);
        Assert.Same(MyPunctuatorAndOperator.Equal, _output.Tokens[2].Type);

        Assert.True(_channel.Consume("==,", _output));
        Assert.Equal("==", _output.Tokens[3].Value);
        Assert.Same(MyPunctuatorAndOperator.EqualOp, _output.Tokens[3].Type);

        Assert.True(_channel.Consume("*=,", _output));
        Assert.Equal("*=", _output.Tokens[4].Value);
        Assert.Same(MyPunctuatorAndOperator.MulAssign, _output.Tokens[4].Type);

        Assert.True(_channel.Consume("!=,", _output));
        Assert.Equal("!=", _output.Tokens[5].Value);
        Assert.Same(MyPunctuatorAndOperator.NotEqual, _output.Tokens[5].Type);

        Assert.False(_channel.Consume("!", _output));
    }

    [Fact]
    public void TestNotConsumeWord()
    {
        Assert.False(_channel.Consume("word", _output));
    }

    record MyPunctuatorAndOperator : ITokenType
    {
        public readonly static MyPunctuatorAndOperator Star = new(nameof(Star), "*");
        public readonly static MyPunctuatorAndOperator Colon = new(nameof(Colon), ",");
        public readonly static MyPunctuatorAndOperator Equal = new(nameof(Equal), "=");
        public readonly static MyPunctuatorAndOperator EqualOp = new(nameof(EqualOp), "==");
        public readonly static MyPunctuatorAndOperator MulAssign = new(nameof(MulAssign), "*=");
        public readonly static MyPunctuatorAndOperator NotEqual = new(nameof(NotEqual), "!=");

        public readonly static IEnumerable<ITokenType> Values = new[]
        {
            Star, Colon, Equal, EqualOp, MulAssign, NotEqual
        };

        private MyPunctuatorAndOperator(string name, string value)
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
