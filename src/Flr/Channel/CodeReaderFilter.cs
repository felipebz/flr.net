namespace Flr.Channel;

public abstract class CodeReaderFilter<T>
{
    protected CodeReaderFilter()
    {
    }

    protected CodeReaderFilter(T output)
    {
        Output = output;
    }

    public T Output { get; set; } = default!;
    public virtual TextReader Reader { get; set; } = null!;
    public CodeReaderConfiguration Configuration { get; set; } = null!;

    public abstract int Read(char[] filteredBuffer, int offset, int length);
}