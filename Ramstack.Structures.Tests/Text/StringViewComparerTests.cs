namespace Ramstack.Text;

[TestFixture]
public class StringViewComparerTests
{
    [Test]
    public void Equals_Ordinal()
    {
        var comparer = StringViewComparer.Ordinal;

        Assert.That(comparer.Equals("test", "test"), Is.True);
        Assert.That(comparer.Equals("test", "Test"), Is.False);
    }

    [Test]
    public void Equals_OrdinalIgnoreCase()
    {
        var comparer = StringViewComparer.OrdinalIgnoreCase;
        Assert.That(comparer.Equals("test", "Test"), Is.True);
    }

    [Test]
    public void Equals_CultureCulture()
    {
        var comparer = StringViewComparer.CurrentCulture;
        Assert.That(comparer.Equals("test", "test"), Is.True);
        Assert.That(comparer.Equals("test", "Test"), Is.False);
    }

    [Test]
    public void Equals_CultureCultureIgnoreCase()
    {
        var comparer = StringViewComparer.CurrentCultureIgnoreCase;
        Assert.That(comparer.Equals("test", "Test"), Is.True);
    }

    [Test]
    public void Equals_InvariantCulture()
    {
        var comparer = StringViewComparer.InvariantCulture;
        Assert.That(comparer.Equals("test", "test"), Is.True);
        Assert.That(comparer.Equals("test", "Test"), Is.False);
    }

    [Test]
    public void Equals_InvariantCultureIgnoreCase()
    {
        var comparer = StringViewComparer.CurrentCultureIgnoreCase;
        Assert.That(comparer.Equals("test", "Test"), Is.True);
    }

    [Test]
    public void Compare_Ordinal()
    {
        var comparer = StringViewComparer.Ordinal;
        Assert.That(comparer.Compare("test", "test"), Is.Zero);
        Assert.That(comparer.Compare("test", "Test"), Is.Not.Zero);
    }

    [Test]
    public void Compare_OrdinalIgnoreCase()
    {
        var comparer = StringViewComparer.OrdinalIgnoreCase;
        Assert.That(comparer.Compare("test", "Test"), Is.Zero);
    }

    [Test]
    public void FromComparison_Ordinal()
    {
        var comparer = StringViewComparer.FromComparison(StringComparison.Ordinal);
        Assert.That(comparer, Is.SameAs(StringViewComparer.Ordinal));
    }

    [Test]
    public void FromComparison_OrdinalIgnoreCase()
    {
        var comparer = StringViewComparer.FromComparison(StringComparison.OrdinalIgnoreCase);
        Assert.That(comparer, Is.SameAs(StringViewComparer.OrdinalIgnoreCase));
    }

    [Test]
    public void FromComparison_CurrentCulture()
    {
        var comparer = StringViewComparer.CurrentCulture;
        Assert.That(comparer.Equals("test", "test"), Is.True);
        Assert.That(comparer.Equals("test", "Test"), Is.False);
    }

    [Test]
    public void FromComparison_CurrentCultureIgnoreCase()
    {
        var comparer = StringViewComparer.CurrentCultureIgnoreCase;
        Assert.That(comparer.Equals("test", "Test"), Is.True);
    }

    [Test]
    public void FromComparison_InvariantCulture()
    {
        var comparer = StringViewComparer.FromComparison(StringComparison.InvariantCulture);
        Assert.That(comparer, Is.SameAs(StringViewComparer.InvariantCulture));
    }

    [Test]
    public void FromComparison_InvariantCultureIgnoreCase()
    {
        var comparer = StringViewComparer.FromComparison(StringComparison.InvariantCultureIgnoreCase);
        Assert.That(comparer, Is.SameAs(StringViewComparer.InvariantCultureIgnoreCase));
    }

    [Test]
    public void FromComparison_InvalidComparison()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => StringViewComparer.FromComparison((StringComparison)999));
    }

    [Test]
    public void Create()
    {
        var c1 = StringViewComparer.Create(CultureInfo.InvariantCulture, false);
        var c2 = StringViewComparer.Create(CultureInfo.InvariantCulture, CompareOptions.None);

        Assert.That(c1.Equals("test", "test"), Is.True);
        Assert.That(c2.Equals("test", "test"), Is.True);

        Assert.That(c1.Equals("test", "TEST"), Is.False);
        Assert.That(c2.Equals("test", "TEST"), Is.False);
    }

    [Test]
    public void Create_IgnoreCase()
    {
        var c1 = StringViewComparer.Create(CultureInfo.InvariantCulture, true);
        var c2 = StringViewComparer.Create(CultureInfo.InvariantCulture, CompareOptions.IgnoreCase);

        Assert.That(c1.Equals("test", "TEST"), Is.True);
        Assert.That(c2.Equals("test", "TEST"), Is.True);
    }
}
