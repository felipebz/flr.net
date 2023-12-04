using System.Buffers;

namespace Flr.Api;

public class Token
{
    private static readonly SearchValues<char> LineBreaks = SearchValues.Create("\n\r");

    private List<Trivia>? _trivias;

    public ITokenType Type { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public string OriginalValue { get; private set; } = null!;
    public int Line { get; private set; }
    public int Column { get; private set; }
    public int EndLine { get; private set; }
    public int EndColumn { get; private set; }
    public IList<Trivia> Trivia { get => _trivias ?? new List<Trivia>(); }
    public bool GeneratedCode { get; private set; }
    public bool HasTrivia => _trivias is { Count: > 0 };

    private void Build()
    {
        var lastLineLength = 0;

        var valueSpan = Value.AsSpan();
        var pos = valueSpan.IndexOfAny(LineBreaks);
        while (pos != -1)
        {
            EndLine++;

            int charsToAdvance = valueSpan[pos + 1] == '\n' ? 2 : 1;
            valueSpan = valueSpan[(pos + charsToAdvance)..];

            pos = valueSpan.IndexOfAny(LineBreaks);
            lastLineLength = valueSpan.Length;
        }

        var endLineOffset = EndLine != Line ? lastLineLength : Column + Value.Length;
        EndColumn = endLineOffset;
    }

    public bool IsOnSameLineAs(Token token)
    {
        return Line == token.Line;
    }

    public override string ToString()
    {
        return $"{Type} {Value}";
    }

    public static TokenBuilder Builder()
    {
        return new TokenBuilder();
    }

    public static TokenBuilder Builder(Token token)
    {
        return new TokenBuilder(token);
    }

    public class TokenBuilder
    {
        private readonly Token _result = new();

        public TokenBuilder()
        {
        }

        public TokenBuilder(Token token)
        {
            _result.Type = token.Type;
            _result.Value = token.Value;
            _result.OriginalValue = token.OriginalValue;
            _result.Line = token.Line;
            _result.Column = token.Column;
            _result._trivias = token._trivias;
            _result.GeneratedCode = token.GeneratedCode;
        }

        public TokenBuilder WithType(ITokenType type)
        {
            _result.Type = type;
            return this;
        }

        public TokenBuilder WithValue(string value)
        {
            _result.Value = value;
            return this;
        }

        public TokenBuilder WithValueAndOriginalValue(string sameValue)
        {
            _result.Value = sameValue;
            _result.OriginalValue = sameValue;
            return this;
        }

        public TokenBuilder WithValueAndOriginalValue(string value, string originalValue)
        {
            _result.Value = value;
            _result.OriginalValue = originalValue;
            return this;
        }

        public TokenBuilder WithLine(int line)
        {
            _result.Line = line;
            return this;
        }

        public TokenBuilder WithColumn(int column)
        {
            _result.Column = column;
            return this;
        }

        public TokenBuilder WithGeneratedCode(bool generatedCode)
        {
            _result.GeneratedCode = generatedCode;
            return this;
        }

        public TokenBuilder WithTrivia(IEnumerable<Trivia> trivia)
        {
            _result._trivias = trivia.ToList();
            return this;
        }

        public TokenBuilder AddTrivia(Trivia trivia)
        {
            _result._trivias ??= new List<Trivia>();
            _result._trivias.Add(trivia);
            return this;
        }

        public Token Build()
        {
            _result.Build();
            return _result;
        }
    }
}
