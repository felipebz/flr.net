namespace Flr.Api;

public class AstNode(AstNodeType type, string name, Token? tokenOrNull)
{
    private int _childIndex;
    
    public AstNodeType Type { get; } = type;
    public string Name { get; } = name;
    public Token? TokenOrNull { get; } = tokenOrNull;
    public Token Token => TokenOrNull ?? throw new ArgumentNullException(nameof(tokenOrNull));
    public IList<AstNode> Children { get; } = new List<AstNode>();
    public AstNode? ParentOrNull { get; private set; }
    public AstNode Parent => ParentOrNull ?? throw new ArgumentNullException(nameof(ParentOrNull));
    public int FromIndex { get; private set; }
    public int ToIndex { get; private set; }
    public int NumberOfChildren => Children.Count;
    public bool HasChildren => Children.Any();
    public bool HasToBeSkippedFromAst { get; }
    
    public AstNode? NextAstNodeOrNull => NextSiblingOrNull ?? ParentOrNull?.NextAstNodeOrNull;
    public AstNode NextAstNode => NextAstNodeOrNull ?? throw new ArgumentNullException(nameof(NextAstNodeOrNull));
    
    public AstNode? PreviousAstNodeOrNull => PreviousSiblingOrNull ?? ParentOrNull?.PreviousAstNodeOrNull;
    public AstNode PreviousAstNode => PreviousAstNodeOrNull ?? throw new ArgumentNullException(nameof(PreviousAstNodeOrNull));

    public AstNode? NextSiblingOrNull
    {
        get
        {
            var parent = ParentOrNull;
            if (parent != null && parent.NumberOfChildren > _childIndex + 1)
            {
                return parent.Children[_childIndex + 1];
            }

            return null;
        }
    }
    public AstNode NextSibling => NextSiblingOrNull ?? throw new ArgumentNullException(nameof(NextSiblingOrNull));
    
    public AstNode? PreviousSiblingOrNull
    {
        get
        {
            var parent = ParentOrNull;
            if (parent != null && _childIndex > 0)
            {
                return parent.Children[_childIndex - 1];
            }

            return null;
        }
    }
    public AstNode PreviousSibling => PreviousSiblingOrNull ?? throw new ArgumentNullException(nameof(PreviousSiblingOrNull));
    
    public string TokenValue => TokenOrNull?.Value ?? "";
    public string TokenOriginalValue => TokenOrNull?.OriginalValue ?? "";
    public int TokenLine => TokenOrNull?.Line ?? -1;
    public int TokenColumn => TokenOrNull?.Column ?? -1;
    public bool HasToken => TokenOrNull != null;
    public AstNode? FirstChildOrNull => Children.FirstOrDefault();
    public AstNode FirstChild => Children.First();
    public AstNode? LastChildOrNull => Children.LastOrDefault();
    public AstNode LastChild => Children.Last();

    public AstNode(Token token) : this(token.Type, token.Value, token)
    {
    }

    public void AddChild(AstNode child)
    {
        if (child.HasToBeSkippedFromAst)
        {
            foreach (var subChild in child.Children)
            {
                AddChildToList(subChild);
            }
        }
        else
        {
            AddChildToList(child);
        }
    }
    
    public bool Is(params AstNodeType[] types)
    {
        return types.Contains(Type);
    }
    
    public bool IsNot(params AstNodeType[] types)
    {
        return !Is(types);
    }
    
    public AstNode? GetFirstChildOrNull(params AstNodeType[] types)
    {
        return Children.FirstOrDefault(child => types.Length == 0 || child.Is(types));
    }
    
    public AstNode GetFirstChild(params AstNodeType[] types)
    {
        return Children.First(child => types.Length == 0 || child.Is(types));
    }
    
    public AstNode? GetLastChildOrNull(params AstNodeType[] types)
    {
        return Children.LastOrDefault(child => types.Length == 0 || child.Is(types));
    }
    
    public AstNode GetLastChild(params AstNodeType[] types)
    {
        return Children.Last(child => types.Length == 0 || child.Is(types));
    }
    
    public bool HasDirectChildren(params AstNodeType[] types)
    {
        return Children.Any(child => types.Length == 0 || child.Is(types));
    }
    
    public IEnumerable<AstNode> GetChildren(params AstNodeType[] types)
    {
        return Children.Where(child => types.Length == 0 || child.Is(types));
    }
    
    public IEnumerable<AstNode> GetDescendants(params AstNodeType[] types)
    {
        foreach (var child in Children)
        {
            if (child.Is(types))
            {
                yield return child;
            }

            var node = child.GetFirstDescendantOrNull(types);
            if (node != null)
            {
                yield return node;
            }
        }
    }
    
    public AstNode? GetFirstDescendantOrNull(params AstNodeType[] types)
    {
        return GetDescendants(types).FirstOrDefault();
    }
    
    public AstNode GetFirstDescendant(params AstNodeType[] types)
    {
        return GetDescendants(types).First();
    }
    
    public bool HasDescendant(params AstNodeType[] types)
    {
        return GetDescendants(types).Any();
    }

    private void AddChildToList(AstNode child)
    {
        Children.Add(child);
        child.ParentOrNull = this;
        child._childIndex = Children.Count - 1;
    }
}