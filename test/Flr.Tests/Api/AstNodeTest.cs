using Flr.Api;

namespace Flr.Tests.Api;

public class AstNodeTest
{
    [Fact]
    public void Test()
    {
        var a = new NodeType();
        var b = new NodeType();
        var c = new NodeType();
        var a1 = new AstNode(a, "a1", null);
        var a2 = new AstNode(a, "a2", null);
        var b1 = new AstNode(b, "b1", null);
        var b2 = new AstNode(b, "b2", null);
        var b3 = new AstNode(b, "b3", null);
        var c1 = new AstNode(c, "c1", null);
        var c2 = new AstNode(c, "c2", null);
        a1.AddChild(a2);
        a2.AddChild(b1);
        a1.AddChild(b2);
        b2.AddChild(c1);
        a1.AddChild(b3);
        a1.AddChild(c2);
        Assert.True(a1.HasChildren);
        Assert.False(c1.HasChildren);
        Assert.Same(a2, a1.FirstChild);
        Assert.Same(c2, a1.LastChild);
        Assert.False(a1.HasDirectChildren(new NodeType()));
        Assert.True(a1.HasDirectChildren(a));
        Assert.True(a1.HasDirectChildren(a, b));
        Assert.Null(a1.GetFirstChildOrNull(new NodeType()));
        Assert.Same(a2, a1.GetFirstChild(a));
        Assert.Same(b2, a1.GetFirstChild(b));
        Assert.Same(a2, a1.GetFirstChild(a, b));
        Assert.Null(a1.GetFirstChildOrNull(new NodeType()));
        Assert.Same(a2, a1.GetLastChild(a));
        Assert.Same(b3, a1.GetLastChild(b));
        Assert.Same(b3, a1.GetLastChild(a, b));
        Assert.Empty(a1.GetChildren(new NodeType()));
        Assert.Equal(new[] { a2 }, a1.GetChildren(a));
        Assert.Equal(new[] { b2, b3 }, a1.GetChildren(b));
        Assert.Equal(new[] { a2, b2, b3 }, a1.GetChildren(a, b));
        Assert.False(a1.HasDescendant(new NodeType()));
        Assert.True(a1.HasDescendant(a));
        Assert.True(a1.HasDescendant(a, b));
        Assert.Null(a1.GetFirstDescendantOrNull(new NodeType()));
        Assert.Same(a2, a1.GetFirstDescendant(a));
        Assert.Same(b1, a1.GetFirstDescendant(b));
        Assert.Same(a2, a1.GetFirstDescendant(a, b));
        Assert.Empty(a1.GetDescendants(new NodeType()));
    }

    private class NodeType : IAstNodeType;
}
