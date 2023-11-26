using System.Text;
using System.Text.RegularExpressions;

namespace Flr.Channel;

public abstract class RegexChannel<T>(Regex myRegex) : IChannel<T>
{
    private readonly StringBuilder _builder = new();

    protected RegexChannel(string regex) : this(new Regex($"^{regex}", RegexOptions.Compiled))
    {
    }

    public bool Consume(CodeReader code, T output)
    {
        if (code.PopTo(myRegex, _builder) > char.MinValue)
        {
            Consume(_builder, output);
            _builder.Clear();
            return true;
        }

        return false;
    }

    protected abstract void Consume(StringBuilder builder, T output);
}
