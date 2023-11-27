using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using Flr.Api;
using Flr.Channel;

namespace Flr.Impl.Channel;

public class IdentifierAndKeywordChannel
    (Regex regex, bool caseSensitive, IEnumerable<ITokenType> keywordSets) : IChannel<LexerOutput>
{
    private readonly Dictionary<string, ITokenType> _keywords = keywordSets
        .ToDictionary(x => caseSensitive ? x.Value : x.Value.ToUpper());

    public bool Consume(CodeReader code, LexerOutput output)
    {
        var builder = new StringBuilder();
        if (code.PopTo(regex, builder) <= 0)
        {
            return false;
        }

        Debug.Assert(code.PreviousCursor != null, "code.PreviousCursor != null");

        var word = builder.ToString();
        var wordOriginal = word;
        if (!caseSensitive)
        {
            word = word.ToUpper();
        }

        _keywords.TryGetValue(word, out var keywordType);

        var token = Token.Builder()
            .WithType(keywordType ?? GenericTokenType.Identifier)
            .WithValueAndOriginalValue(word, wordOriginal)
            .WithLine(code.PreviousCursor.Line)
            .WithColumn(code.PreviousCursor.Column)
            .Build();
        output.AddToken(token);
        builder.Clear();
        return true;
    }
}
