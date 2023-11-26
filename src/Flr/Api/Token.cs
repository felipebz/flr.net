namespace Flr.Api;

public class Token
{
    public ITokenType Type { get; }
    public string Value { get; }
    public string OriginalValue { get; }
    public int Line { get; }
    public int Column { get; }
    public int EndLine { get; }
    public int EndColumn { get; }
    public IList<Trivia> Trivia { get; }
    public bool GeneratedCode { get; }
    public bool HasTrivia => Trivia.Any();

    private Token(TokenBuilder builder)
    {
        Type = builder.Type;
        Value = builder.Value;
        OriginalValue = builder.OriginalValue;
        Line = builder.Line;
        Column = builder.Column;
        Trivia = builder.Trivia;
        GeneratedCode = builder.GeneratedCode;

        var lastLineLength = 0;
        var lineCount = 1;

        if (Value.IndexOf('\n') != -1 || Value.IndexOf('\r') != -1)
        {
            var lines = Value.Split('\n', '\r');
            lineCount = lines.Length > 1 ? lines.Length : 1;
            lastLineLength = lines[^1].Length;
        }

        EndLine = Line + lineCount - 1;
        var endLineOffset = EndLine != Line ? lastLineLength : Column + Value.Length;
        EndColumn = endLineOffset;
    }

    internal static Token CreateInstance(TokenBuilder builder)
    {
        return new Token(builder);
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
        public ITokenType Type { get; set; } = null!;
        public string Value { get; set; } = "";
        public string OriginalValue { get; set; } = "";
        public int Line { get; set; } = 0;
        public int Column { get; set; } = -1;
        public IList<Trivia> Trivia { get; set; } = new List<Trivia>();
        public bool GeneratedCode { get; set; } = false;

        public TokenBuilder()
        {
        }

        public TokenBuilder(Token token)
        {
            Type = token.Type;
            Value = token.Value;
            OriginalValue = token.OriginalValue;
            Line = token.Line;
            Column = token.Column;
            Trivia = token.Trivia;
            GeneratedCode = token.GeneratedCode;
        }

        public TokenBuilder WithType(ITokenType type)
        {
            Type = type;
            return this;
        }

        public TokenBuilder WithValue(string value)
        {
            Value = value;
            return this;
        }

        public TokenBuilder WithValueAndOriginalValue(string sameValue)
        {
            Value = sameValue;
            OriginalValue = sameValue;
            return this;
        }

        public TokenBuilder WithValueAndOriginalValue(string value, string originalValue)
        {
            Value = value;
            OriginalValue = originalValue;
            return this;
        }

        public TokenBuilder WithLine(int line)
        {
            Line = line;
            return this;
        }

        public TokenBuilder WithColumn(int column)
        {
            Column = column;
            return this;
        }

        public TokenBuilder WithGeneratedCode(bool generatedCode)
        {
            GeneratedCode = generatedCode;
            return this;
        }

        public TokenBuilder WithTrivia(IList<Trivia> trivia)
        {
            Trivia = trivia.ToList();
            return this;
        }

        public TokenBuilder AddTrivia(Trivia trivia)
        {
            Trivia.Add(trivia);
            return this;
        }

        public Token Build()
        {
            return CreateInstance(this);
        }
    }
}
