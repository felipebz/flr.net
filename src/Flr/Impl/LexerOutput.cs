using Flr.Api;

namespace Flr.Impl;

public class LexerOutput(Uri? uri = null)
{
    private readonly List<Trivia> _trivias = new();
    private readonly List<Token> _tokens = new();

    public Uri Uri { get; } = uri ?? new Uri("tests://unittest");
    public IReadOnlyList<Token> Tokens => _tokens.AsReadOnly();

    public void AddTrivia(Trivia trivia)
    {
        _trivias.Add(trivia);
    }

    public void AddToken(Token token)
    {
        if (_trivias.Count > 0 || token.HasTrivia)
        {
            token = Token.Builder(token).WithTrivia(_trivias).Build();
            _trivias.Clear();
        }

        _tokens.Add(token);
    }
}
