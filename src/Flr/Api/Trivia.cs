using System.Text;

namespace Flr.Api;

public class Trivia
{
    private readonly TriviaKind _kind;
    public Token[] Tokens { get; }
    public Token Token => Tokens[0];
    public bool IsComment => _kind == TriviaKind.Comment;
    public bool IsSkippedText => _kind == TriviaKind.SkippedText;

    public Trivia(TriviaKind kind, params Token[] tokens)
    {
        _kind = kind;
        Tokens = tokens;
    }

    public static Trivia CreateComment(Token commentToken)
    {
        return new Trivia(TriviaKind.Comment, commentToken);
    }

    public static Trivia CreateSkippedText(Token skippedTextToken)
    {
        return new Trivia(TriviaKind.SkippedText, skippedTextToken);
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
        Comment,
        SkippedText,
    }
}
