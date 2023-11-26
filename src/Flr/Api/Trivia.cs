using System.Text;

namespace Flr.Api;

public class Trivia
{
    private readonly TriviaKind _kind;
    public Token[] Tokens { get; }
    public Token Token => Tokens[0];
    public bool IsComment => _kind == TriviaKind.COMMENT;
    public bool IsSkippedText => _kind == TriviaKind.SKIPPED_TEXT;

    public Trivia(TriviaKind kind, params Token[] tokens)
    {
        _kind = kind;
        Tokens = tokens;
    }

    public override string ToString()
    {
        switch (Tokens.Length)
        {
            case 0:
                return $"TRIVIA kind={_kind}";
            case 1:
                return $"TRIVIA kind={_kind} line={Token.Line} type={Token.Type} value={Token.OriginalValue}";
            default:
                var sb = new StringBuilder();
                foreach (var token in Tokens)
                {
                    sb.Append(token.OriginalValue);
                    sb.Append(' ');
                }

                return $"TRIVIA kind={_kind} value = {sb}";
        }
    }

    public enum TriviaKind
    {
        COMMENT,
        SKIPPED_TEXT,
    }
}