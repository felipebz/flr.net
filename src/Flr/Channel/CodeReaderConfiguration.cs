namespace Flr.Channel;

public class CodeReaderConfiguration
{
    private const int DefaultTabWidth = 1;

    public int TabWidth { get; set; } = DefaultTabWidth;

    public IList<CodeReaderFilter<object>> Filters { get; } = new List<CodeReaderFilter<object>>();

    public CodeReaderConfiguration CloneWithoutCodeReaderFilters()
    {
        return new CodeReaderConfiguration { TabWidth = TabWidth };
    }
}
