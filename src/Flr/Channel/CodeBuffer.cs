namespace Flr.Channel;

public class CodeBuffer
{
    private readonly int tabWidth;
    protected readonly Cursor cursor = new();

    public CodeBuffer(TextReader initialCodeReader, CodeReaderConfiguration configuration)
    {
        using (initialCodeReader)
        {
            LastChar = char.MinValue;
            
            tabWidth = configuration.TabWidth;
            var filteredReader = configuration.Filters
                .Aggregate(initialCodeReader, (current, codeReaderFilter) =>
                    new Filter(current, codeReaderFilter, configuration));

            using (filteredReader)
            {
                Buffer = Read(filteredReader);
            }
            
        }
    }

    public CodeBuffer(string code, CodeReaderConfiguration configuration) : this(new StringReader(code), configuration)
    {
    }
    
    public string Buffer { get; }
    
    public int BufferPosition { get; private set; }
    
    public char LastChar { get; private set; }
    
    public int LinePosition
    {
        get => cursor.Line;
        set => cursor.Line = value;
    }

    public int ColumnPosition
    {
        get => cursor.Column;
        set => cursor.Column = value;
    }
    
    public int Length => Buffer.Length - BufferPosition;

    private static string Read(TextReader reader)
    {
        return reader.ReadToEnd();
    }
    
    public char Pop()
    {
        if (BufferPosition >= Buffer.Length)
        {
            return char.MinValue;
        }
        var character = Buffer[BufferPosition++];
        UpdateCursorPosition(character);
        LastChar = character;
        return character;
    }

    private void UpdateCursorPosition(char character)
    {
        switch (character)
        {
            case '\n':
            case '\r' when Peek() != '\n':
                cursor.Line++;
                cursor.Column = 0;
                break;
            case '\t':
                cursor.Column += tabWidth;
                break;
            default:
                cursor.Column++;
                break;
        }
    }

    public char Peek()
    {
        return CharAt(0);
    }

    public char CharAt(int i)
    {
        return BufferPosition + i >= Buffer.Length ? char.MinValue : Buffer[BufferPosition + i];
    }

    public class Cursor
    {
        public int Line { get; set; } = 1;
        public int Column { get; set; }
        
        public Cursor Clone()
        {
            return new Cursor { Line = Line, Column = Column };
        }
    }
    
    private class Filter : TextReader
    {
        private readonly CodeReaderFilter<object> _codeReaderFilter;

        public Filter(TextReader filteredReader, CodeReaderFilter<object> codeReaderFilter, CodeReaderConfiguration configuration)
        {
            _codeReaderFilter = codeReaderFilter;
            _codeReaderFilter.Configuration = configuration.CloneWithoutCodeReaderFilters();
            _codeReaderFilter.Reader = filteredReader;
        }

        public override int Read()
        {
            throw new NotSupportedException();
        }
        
        public override int Read(char[] buffer, int offset, int count)
        {
            return _codeReaderFilter.Read(buffer, offset, count);
        }
    }

    public char this[int i] => CharAt(i);
}