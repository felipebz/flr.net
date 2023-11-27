using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Flr.Api;
using Flr.Impl;
using Flr.Impl.Channel;

namespace Flr.Tests.Samples;

public static partial class MiniCLexer
{
    [GeneratedRegex("^[a-zA-Z]([a-zA-Z0-9_]*[a-zA-Z0-9])?")]
    private static partial Regex IdentifierRegex();

    [GeneratedRegex("^[0-9]+")]
    private static partial Regex IntegerRegex();

    [GeneratedRegex("(?s)^/\\*.*?\\*/")]
    private static partial Regex CommentRegex();

    [GeneratedRegex("^[ \t\r\n]+")]
    private static partial Regex DiscardRegex();

    public static Lexer Create()
    {
        return Lexer.Builder()
            .WithFailIfNoChannelToConsumeOneCharacter(true)
            .AddChannel(new IdentifierAndKeywordChannel(IdentifierRegex(), true, Keywords.Values))
            .AddChannel(new RegexChannel(Literals.Integer, IntegerRegex()))
            .AddChannel(new CommentRegexChannel(CommentRegex()))
            .AddChannel(new PunctuatorChannel(Punctuators.Values))
            .AddChannel(new DiscardChannel(DiscardRegex()))
            .Build();
    }

    public record Literals : ITokenType
    {
        public readonly static Literals Integer = new();

        private Literals([CallerMemberName] string name = null!)
        {
            Name = name;
            Value = name;
        }

        public string Name { get; }
        public string Value { get; }

        public bool HasToBeSkippedFromAst(AstNode? node)
        {
            return false;
        }
    }

    public record Punctuators : ITokenType
    {
        public readonly static List<ITokenType> Values = new();

        public readonly static Punctuators ParenL = new("(");
        public readonly static Punctuators ParenR = new(")");
        public readonly static Punctuators BraceL = new("{");
        public readonly static Punctuators BraceR = new("}");
        public readonly static Punctuators Eq = new("=");
        public readonly static Punctuators Comma = new(",");
        public readonly static Punctuators Semicolon = new(";");
        public readonly static Punctuators Add = new("+");
        public readonly static Punctuators Sub = new("-");
        public readonly static Punctuators Mul = new("*");
        public readonly static Punctuators Div = new("/");
        public readonly static Punctuators EqEq = new("==");
        public readonly static Punctuators Ne = new("!=");
        public readonly static Punctuators Lt = new("<");
        public readonly static Punctuators Lte = new("<=");
        public readonly static Punctuators Gt = new(">");
        public readonly static Punctuators Gte = new(">=");
        public readonly static Punctuators Inc = new("++");
        public readonly static Punctuators Dec = new("--");
        public readonly static Punctuators Hash = new("#");

        private Punctuators(string value, [CallerMemberName] string name = null!)
        {
            Name = name;
            Value = value;
            Values.Add(this);
        }

        public string Name { get; }
        public string Value { get; }

        public bool HasToBeSkippedFromAst(AstNode? node)
        {
            return false;
        }
    }

    public record Keywords : ITokenType
    {
        public readonly static List<ITokenType> Values = new();

        public readonly static Keywords Struct = new("struct");
        public readonly static Keywords Int = new("int");
        public readonly static Keywords Void = new("void");
        public readonly static Keywords Return = new("return");
        public readonly static Keywords If = new("if");
        public readonly static Keywords Else = new("else");
        public readonly static Keywords While = new("while");
        public readonly static Keywords Continue = new("continue");
        public readonly static Keywords Break = new("break");

        private Keywords(string value, [CallerMemberName] string name = null!)
        {
            Name = name;
            Value = value;
            Values.Add(this);
        }

        public string Name { get; }
        public string Value { get; }

        public bool HasToBeSkippedFromAst(AstNode? node)
        {
            return false;
        }
    }
}
