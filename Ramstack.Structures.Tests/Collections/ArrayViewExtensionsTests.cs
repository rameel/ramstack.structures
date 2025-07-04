namespace Ramstack.Collections;

#if NET9_0_OR_GREATER

[TestFixture]
public class ArrayViewExtensionsTests
{
    [Test]
    public void List_AsView_Empty()
    {
        var list = new List<string>();
        var view = list.AsView();

        Assert.That(view.Length, Is.EqualTo(list.Count));
        Assert.That(view, Is.EquivalentTo(list));
    }

    [Test]
    public void List_AsView()
    {
        var list = new List<string> { "apple", "banana", "cherry", "kiwi", "elderberry" };
        var view = list.AsView();

        Assert.That(view.Length, Is.EqualTo(list.Count));
        Assert.That(view, Is.EquivalentTo(list));
    }

}

#endif
