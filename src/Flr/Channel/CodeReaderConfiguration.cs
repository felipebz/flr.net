namespace Flr.Channel;

public class CodeReaderConfiguration
{
    private const int DEFAULT_TAB_WIDTH = 1;
    
    public int TabWidth { get; set; } = DEFAULT_TAB_WIDTH;
    
    public IList<CodeReaderFilter<object>> Filters { get; } = new List<CodeReaderFilter<object>>();

    public CodeReaderConfiguration CloneWithoutCodeReaderFilters()
    {
        return new CodeReaderConfiguration
        {
            TabWidth = TabWidth
        };
    }
}