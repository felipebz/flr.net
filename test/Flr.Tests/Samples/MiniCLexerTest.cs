using Flr.Api;
using Flr.Impl;

namespace Flr.Tests.Samples;

public class MiniCLexerTest
{
    private readonly Lexer _lexer = MiniCLexer.Create();

    [Fact]
    public void LexIdentifiers()
    {
        Assert.Contains(_lexer.Lex("abc"),
            token => Matches(token, GenericTokenType.Identifier, "abc"));
        Assert.Contains(_lexer.Lex("abc0"),
            token => Matches(token, GenericTokenType.Identifier, "abc0"));
        Assert.Contains(_lexer.Lex("abc_0"),
            token => Matches(token, GenericTokenType.Identifier, "abc_0"));
        Assert.Contains(_lexer.Lex("i"),
            token => Matches(token, GenericTokenType.Identifier, "i"));
    }

    [Fact]
    public void LexIntegers()
    {
        Assert.Contains(_lexer.Lex("0"),
            token => Matches(token, MiniCLexer.Literals.Integer, "0"));
        Assert.Contains(_lexer.Lex("000"),
            token => Matches(token, MiniCLexer.Literals.Integer, "000"));
        Assert.Contains(_lexer.Lex("1234"),
            token => Matches(token, MiniCLexer.Literals.Integer, "1234"));
    }

    [Fact]
    public void LexKeywords()
    {
        Assert.Contains(_lexer.Lex("int"), token => Matches(token, MiniCLexer.Keywords.Int));
        Assert.Contains(_lexer.Lex("void"), token => Matches(token, MiniCLexer.Keywords.Void));
        Assert.Contains(_lexer.Lex("return"), token => Matches(token, MiniCLexer.Keywords.Return));
        Assert.Contains(_lexer.Lex("if"), token => Matches(token, MiniCLexer.Keywords.If));
        Assert.Contains(_lexer.Lex("else"), token => Matches(token, MiniCLexer.Keywords.Else));
        Assert.Contains(_lexer.Lex("while"), token => Matches(token, MiniCLexer.Keywords.While));
        Assert.Contains(_lexer.Lex("break"), token => Matches(token, MiniCLexer.Keywords.Break));
        Assert.Contains(_lexer.Lex("continue"), token => Matches(token, MiniCLexer.Keywords.Continue));
        Assert.Contains(_lexer.Lex("struct"), token => Matches(token, MiniCLexer.Keywords.Struct));
    }

    [Fact]
    public void LexComments()
    {
        Assert.Contains(_lexer.Lex("/*test*/"), token => CommentMatches(token, "/*test*/"));
        Assert.Contains(_lexer.Lex("/*test*/*/"), token => CommentMatches(token, "/*test*/"));
        Assert.Contains(_lexer.Lex("/*test/* /**/"), token => CommentMatches(token, "/*test/* /**/"));
        Assert.Contains(_lexer.Lex("/*test1\ntest2\ntest3*/"),
            token => CommentMatches(token, "/*test1\ntest2\ntest3*/"));
    }

    [Fact]
    public void LexPunctuators()
    {
        Assert.Contains(_lexer.Lex("("), token => Matches(token, MiniCLexer.Punctuators.ParenL));
        Assert.Contains(_lexer.Lex(")"), token => Matches(token, MiniCLexer.Punctuators.ParenR));
        Assert.Contains(_lexer.Lex("{"), token => Matches(token, MiniCLexer.Punctuators.BraceL));
        Assert.Contains(_lexer.Lex("}"), token => Matches(token, MiniCLexer.Punctuators.BraceR));
        Assert.Contains(_lexer.Lex("="), token => Matches(token, MiniCLexer.Punctuators.Eq));
        Assert.Contains(_lexer.Lex(","), token => Matches(token, MiniCLexer.Punctuators.Comma));
        Assert.Contains(_lexer.Lex(";"), token => Matches(token, MiniCLexer.Punctuators.Semicolon));
        Assert.Contains(_lexer.Lex("+"), token => Matches(token, MiniCLexer.Punctuators.Add));
        Assert.Contains(_lexer.Lex("-"), token => Matches(token, MiniCLexer.Punctuators.Sub));
        Assert.Contains(_lexer.Lex("*"), token => Matches(token, MiniCLexer.Punctuators.Mul));
        Assert.Contains(_lexer.Lex("/"), token => Matches(token, MiniCLexer.Punctuators.Div));
        Assert.Contains(_lexer.Lex("<"), token => Matches(token, MiniCLexer.Punctuators.Lt));
        Assert.Contains(_lexer.Lex("<="), token => Matches(token, MiniCLexer.Punctuators.Lte));
        Assert.Contains(_lexer.Lex(">"), token => Matches(token, MiniCLexer.Punctuators.Gt));
        Assert.Contains(_lexer.Lex(">="), token => Matches(token, MiniCLexer.Punctuators.Gte));
        Assert.Contains(_lexer.Lex("=="), token => Matches(token, MiniCLexer.Punctuators.EqEq));
        Assert.Contains(_lexer.Lex("!="), token => Matches(token, MiniCLexer.Punctuators.Ne));
        Assert.Contains(_lexer.Lex("++"), token => Matches(token, MiniCLexer.Punctuators.Inc));
        Assert.Contains(_lexer.Lex("--"), token => Matches(token, MiniCLexer.Punctuators.Dec));
    }

    private static bool Matches(Token token, ITokenType tokenType, string value)
    {
        return ReferenceEquals(token.Type, tokenType) && token.Value == value;
    }

    private static bool Matches(Token token, ITokenType tokenType)
    {
        return ReferenceEquals(token.Type, tokenType);
    }

    private static bool CommentMatches(Token token, string value)
    {
        return token.HasTrivia && token.Trivia[0].IsComment && token.Trivia[0].Token.Value == value;
    }
}
