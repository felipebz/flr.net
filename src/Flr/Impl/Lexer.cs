using System.Text;

using Flr.Api;
using Flr.Channel;

namespace Flr.Impl;

public class Lexer
{
    private readonly Encoding _encoding;
    private readonly CodeReaderConfiguration _configuration;
    private readonly ChannelDispatcher<LexerOutput> _channelDispatcher;

    private Lexer(LexerBuilder builder)
    {
        _encoding = builder.Encoding;
        _configuration = builder.Configuration;
        _channelDispatcher = builder.ChannelDispatcher;
    }

    public LexerBuilder Builder()
    {
        return new LexerBuilder();
    }

    private IEnumerable<Token> Lex(TextReader reader, LexerOutput output)
    {
        var code = new CodeReader(reader, _configuration);
        try
        {
            _channelDispatcher.Consume(code, output);
            output.AddToken(
                Token.Builder()
                    .WithType(GenericTokenType.Eof)
                    .WithValueAndOriginalValue("EOF")
                    .WithLine(code.LinePosition)
                    .WithColumn(code.ColumnPosition)
                    .Build());
            return output.Tokens;
        }
        catch (Exception e)
        {
            throw new LexerException(
                $"Unable to lex source code at line : {code.LinePosition} and column : {code.ColumnPosition} in file: {output.Uri}",
                e);
        }
    }

    public class LexerBuilder
    {
        private readonly IList<IChannel<LexerOutput>> _channels = new List<IChannel<LexerOutput>>();
        private bool _failIfNoChannelToConsumeOneCharacter;
        public Encoding Encoding { get; private set; } = Encoding.UTF8;
        public CodeReaderConfiguration Configuration { get; } = new();

        public ChannelDispatcher<LexerOutput> ChannelDispatcher
        {
            get
            {
                var builder = ChannelDispatcher<LexerOutput>.Builder().AddChannels(_channels);
                if (_failIfNoChannelToConsumeOneCharacter)
                {
                    builder.FailIfNoChannelToConsumeOneCharacter();
                }

                return builder.Build();
            }
        }

        public LexerBuilder WithEncoding(Encoding encoding)
        {
            Encoding = encoding;
            return this;
        }

        public LexerBuilder AddChannel(IChannel<LexerOutput> channel)
        {
            _channels.Add(channel);
            return this;
        }

        public LexerBuilder WithFailIfNoChannelToConsumeOneCharacter(bool failIfNoChannelToConsumeOneCharacter)
        {
            _failIfNoChannelToConsumeOneCharacter = failIfNoChannelToConsumeOneCharacter;
            return this;
        }

        public Lexer Build()
        {
            return new Lexer(this);
        }
    }
}
