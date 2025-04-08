using System.Collections;

namespace Ramstack.Collections;

[TestFixture]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public class DictionarySlimTests
{
    [Test]
    public void Ctor()
    {
        var dictionary = new DictionarySlim<int, int>();
        Assert.That(dictionary.Count, Is.EqualTo(0));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    public void Ctor_Capacity(int capacity)
    {
        var dictionary = new DictionarySlim<int, int>(capacity);
        Assert.That(dictionary.Capacity, Is.GreaterThanOrEqualTo(capacity));
    }

    [Test]
    public void Ctor_Comparer_ValueType()
    {
        var dictionary1 = new DictionarySlim<int, int>();
        var dictionary2 = new DictionarySlim<int, int>(EqualityComparer<int>.Default);
        var dictionary3 = new DictionarySlim<int, int>(new Int32Comparer());

        Assert.That(dictionary1.Comparer, Is.SameAs(EqualityComparer<int>.Default));
        Assert.That(dictionary2.Comparer, Is.SameAs(EqualityComparer<int>.Default));
        Assert.That(dictionary3.Comparer, Is.InstanceOf<Int32Comparer>());
    }

    [Test]
    public void Ctor_Comparer_ReferenceType()
    {
        var dictionary1 = new DictionarySlim<string, int>();
        var dictionary2 = new DictionarySlim<string, int>(EqualityComparer<string>.Default);
        var dictionary3 = new DictionarySlim<string, int>(StringComparer.Ordinal);
        var dictionary4 = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);

        Assert.That(dictionary1.Comparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(dictionary2.Comparer, Is.SameAs(EqualityComparer<string>.Default));
        Assert.That(dictionary3.Comparer, Is.SameAs(StringComparer.Ordinal));
        Assert.That(dictionary4.Comparer, Is.SameAs(StringComparer.OrdinalIgnoreCase));
    }

    [Test]
    public void Add()
    {
        var dictionary = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary.Add("test-1", 1);
        dictionary.Add("test-2", 2);
        dictionary.Add("test-3", 3);
        dictionary.Add("test-4", 4);
        dictionary.Add("test-5", 5);

        Assert.That(dictionary.Count, Is.EqualTo(5));

        dictionary.Remove("Test-1");
        dictionary.Remove("Test-3");
        dictionary.Remove("Test-5");

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["Test-2"], Is.EqualTo(2));
        Assert.That(dictionary["Test-4"], Is.EqualTo(4));

        dictionary.Add("test-6", 6);
        dictionary.Add("test-7", 7);
        dictionary.Add("test-8", 8);
        dictionary.Add("test-9", 9);

        Assert.That(dictionary.Count, Is.EqualTo(6));

        Assert.That(dictionary["Test-2"], Is.EqualTo(2));
        Assert.That(dictionary["Test-4"], Is.EqualTo(4));
        Assert.That(dictionary["Test-6"], Is.EqualTo(6));
        Assert.That(dictionary["Test-7"], Is.EqualTo(7));
        Assert.That(dictionary["Test-8"], Is.EqualTo(8));
        Assert.That(dictionary["Test-9"], Is.EqualTo(9));

        dictionary.Add("text-1", 1);
        dictionary.Add("text-2", 2);
        dictionary.Add("text-3", 3);
        dictionary.Add("text-4", 4);
        dictionary.Add("text-5", 5);

        Assert.That(dictionary.Count, Is.EqualTo(11));

        Assert.That(dictionary["TEXT-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEXT-2"], Is.EqualTo(2));
        Assert.That(dictionary["TEXT-3"], Is.EqualTo(3));
        Assert.That(dictionary["TEXT-4"], Is.EqualTo(4));
        Assert.That(dictionary["TEXT-5"], Is.EqualTo(5));
    }

    [Test]
    public void Add_IDictionary_Generic()
    {
        IDictionary<string, int> dictionary = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary.Add("test-1", 1);
        dictionary.Add("test-2", 2);
        dictionary.Add("test-3", 3);
        dictionary.Add("test-4", 4);
        dictionary.Add("test-5", 5);

        Assert.That(dictionary.Count, Is.EqualTo(5));

        dictionary.Remove("Test-1");
        dictionary.Remove("Test-3");
        dictionary.Remove("Test-5");

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["Test-2"], Is.EqualTo(2));
        Assert.That(dictionary["Test-4"], Is.EqualTo(4));

        dictionary.Add("test-6", 6);
        dictionary.Add("test-7", 7);
        dictionary.Add("test-8", 8);
        dictionary.Add("test-9", 9);

        Assert.That(dictionary.Count, Is.EqualTo(6));

        Assert.That(dictionary["Test-2"], Is.EqualTo(2));
        Assert.That(dictionary["Test-4"], Is.EqualTo(4));
        Assert.That(dictionary["Test-6"], Is.EqualTo(6));
        Assert.That(dictionary["Test-7"], Is.EqualTo(7));
        Assert.That(dictionary["Test-8"], Is.EqualTo(8));
        Assert.That(dictionary["Test-9"], Is.EqualTo(9));

        dictionary.Add("text-1", 1);
        dictionary.Add("text-2", 2);
        dictionary.Add("text-3", 3);
        dictionary.Add("text-4", 4);
        dictionary.Add("text-5", 5);

        Assert.That(dictionary.Count, Is.EqualTo(11));

        Assert.That(dictionary["TEXT-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEXT-2"], Is.EqualTo(2));
        Assert.That(dictionary["TEXT-3"], Is.EqualTo(3));
        Assert.That(dictionary["TEXT-4"], Is.EqualTo(4));
        Assert.That(dictionary["TEXT-5"], Is.EqualTo(5));
    }

    [Test]
    public void Add_IDictionary()
    {
        IDictionary dictionary = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary.Add("test-1", 1);
        dictionary.Add("test-2", 2);
        dictionary.Add("test-3", 3);
        dictionary.Add("test-4", 4);
        dictionary.Add("test-5", 5);

        Assert.That(dictionary.Count, Is.EqualTo(5));

        dictionary.Remove("Test-1");
        dictionary.Remove("Test-3");
        dictionary.Remove("Test-5");

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["Test-2"], Is.EqualTo(2));
        Assert.That(dictionary["Test-4"], Is.EqualTo(4));

        dictionary.Add("test-6", 6);
        dictionary.Add("test-7", 7);
        dictionary.Add("test-8", 8);
        dictionary.Add("test-9", 9);

        Assert.That(dictionary.Count, Is.EqualTo(6));

        Assert.That(dictionary["Test-2"], Is.EqualTo(2));
        Assert.That(dictionary["Test-4"], Is.EqualTo(4));
        Assert.That(dictionary["Test-6"], Is.EqualTo(6));
        Assert.That(dictionary["Test-7"], Is.EqualTo(7));
        Assert.That(dictionary["Test-8"], Is.EqualTo(8));
        Assert.That(dictionary["Test-9"], Is.EqualTo(9));

        dictionary.Add("text-1", 1);
        dictionary.Add("text-2", 2);
        dictionary.Add("text-3", 3);
        dictionary.Add("text-4", 4);
        dictionary.Add("text-5", 5);

        Assert.That(dictionary.Count, Is.EqualTo(11));

        Assert.That(dictionary["TEXT-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEXT-2"], Is.EqualTo(2));
        Assert.That(dictionary["TEXT-3"], Is.EqualTo(3));
        Assert.That(dictionary["TEXT-4"], Is.EqualTo(4));
        Assert.That(dictionary["TEXT-5"], Is.EqualTo(5));
    }

    [Test]
    public void Set()
    {
        var dictionary = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary["test-1"] = 1;
        dictionary["test-2"] = 2;

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["TEST-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEST-2"], Is.EqualTo(2));

        dictionary["Test-2"] = 5;

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["TEST-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEST-2"], Is.EqualTo(5));
    }

    [Test]
    public void Set_IDictionary_Generic()
    {
        IDictionary<string, int> dictionary = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary["test-1"] = 1;
        dictionary["test-2"] = 2;

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["TEST-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEST-2"], Is.EqualTo(2));

        dictionary["Test-2"] = 5;

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["TEST-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEST-2"], Is.EqualTo(5));
    }

    [Test]
    public void Set_IDictionary()
    {
        IDictionary dictionary = new DictionarySlim<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary["test-1"] = 1;
        dictionary["test-2"] = 2;

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["TEST-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEST-2"], Is.EqualTo(2));

        dictionary["Test-2"] = 5;

        Assert.That(dictionary.Count, Is.EqualTo(2));

        Assert.That(dictionary["TEST-1"], Is.EqualTo(1));
        Assert.That(dictionary["TEST-2"], Is.EqualTo(5));
    }

    [Test]
    public void Remove()
    {
        var list = Enumerable
            .Range(0, 50)
            .Select(_ => Guid.NewGuid().ToString())
            .Select(v => new KeyValuePair<string, string>(v, v))
            .ToList();

        var dictionary = new DictionarySlim<string, string>(list, StringComparer.OrdinalIgnoreCase);

        foreach (var v in list)
        {
            Assert.That(dictionary.Remove(v.Key, out var value), Is.True);
            Assert.That(value, Is.EqualTo(v.Value));
        }

        Assert.That(dictionary.Count, Is.EqualTo(0));

        foreach (var v in list)
        {
            Assert.Throws<KeyNotFoundException>(() => _ = dictionary[v.Key]);
            Assert.Throws<KeyNotFoundException>(() => _ = ((IDictionary<string, string>)dictionary)[v.Key]);
            Assert.Throws<KeyNotFoundException>(() => _ = ((IDictionary)dictionary)[v.Key]);

            Assert.That(dictionary.ContainsKey(v.Key), Is.False);
            Assert.That(dictionary.Remove(v.Key, out var value), Is.False);
            Assert.That(value, Is.Null);
        }
    }

    [Test]
    public void CopyTo()
    {
        var list = Enumerable
            .Range(0, 50)
            .Select(_ => Guid.NewGuid().ToString())
            .Select(v => new KeyValuePair<string, string>(v, v))
            .ToList();

        var dictionary = new DictionarySlim<string, string>(list, StringComparer.OrdinalIgnoreCase);

        var pairs = new KeyValuePair<string, string>[list.Count];
        ((IDictionary<string, string>)dictionary).CopyTo(pairs, 0);
        Assert.That(pairs, Is.EquivalentTo(list));

        var keys = new string[list.Count];
        ((IDictionary<string, string>)dictionary).Keys.CopyTo(keys, 0);
        Assert.That(keys, Is.EquivalentTo(list.Select(v => v.Key)));

        var values = new string[list.Count];
        ((IDictionary<string, string>)dictionary).Values.CopyTo(values, 0);
        Assert.That(values, Is.EquivalentTo(list.Select(v => v.Value)));
    }

    [Test]
    public void CopyTo_InsufficientRoom_ShouldThrow()
    {
        var dictionary = new DictionarySlim<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["test-1"] = "test-1"
        };

        var e1 = Assert.Throws<ArgumentException>(() => dictionary.Keys.CopyTo([]))!;
        Assert.That(e1.Message, Is.EqualTo("Destination too short. (Parameter 'destination')"));

        var e2 = Assert.Throws<ArgumentException>(() => dictionary.Values.CopyTo([]))!;
        Assert.That(e2.Message, Is.EqualTo("Destination too short. (Parameter 'destination')"));

        var e3 = Assert.Throws<ArgumentException>(() => ((IDictionary<string, string>)dictionary).CopyTo([], 0))!;
        Assert.That(e3.Message, Is.EqualTo("Destination too short. (Parameter 'array')"));

        var e4 = Assert.Throws<ArgumentException>(() => ((IDictionary<string, string>)dictionary).Keys.CopyTo([], 0))!;
        Assert.That(e4.Message, Is.EqualTo("Destination too short. (Parameter 'destination')"));

        var e5 = Assert.Throws<ArgumentException>(() => ((IDictionary<string, string>)dictionary).Values.CopyTo([], 0))!;
        Assert.That(e5.Message, Is.EqualTo("Destination too short. (Parameter 'destination')"));
    }

    [Test]
    public void ComplexOperations()
    {
        var list = Enumerable.Range(0, 500).Select(_ => Guid.NewGuid().ToString()).ToList();
        var dictionary = new DictionarySlim<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var v in list)
            dictionary.Add(v, v);

        Assert.That(dictionary.Count, Is.EqualTo(list.Count));

        foreach (var v in list)
        {
            Assert.That(dictionary.ContainsKey(v), Is.True);
            Assert.That(dictionary[v], Is.EqualTo(v));
        }

        ValidateContents();

        for (var i = 499; i >= 0; i--)
        {
            if (i % 3 != 0)
            {
                dictionary.Remove(list[i]);
                list.RemoveAt(i);
            }
        }

        ValidateContents();

        while (list.Count < 500)
        {
            var v = Guid.NewGuid().ToString();
            list.Add(v);
            dictionary.Add(v, v);
        }

        ValidateContents();

        while (list.Count < 700)
        {
            var v = Guid.NewGuid().ToString();
            list.Add(v);
            dictionary.Add(v, v);
        }

        ValidateContents();

        foreach (var v in list)
            dictionary.Remove(v);
        list.Clear();
        ValidateContents();

        while (list.Count < 100)
        {
            var v = Guid.NewGuid().ToString();
            list.Add(v);
            dictionary.Add(v, v);
        }

        ValidateContents();

        dictionary.Clear();
        list.Clear();
        ValidateContents();

        void ValidateContents()
        {
            var expected = new List<string>();
            foreach (var v in dictionary)
                expected.Add(v.Key);

            Assert.That(
                expected.OrderBy(v => v),
                Is.EquivalentTo(list.OrderBy(v => v)));

            expected.Clear();
            foreach (var v in dictionary.Keys)
                expected.Add(v);

            Assert.That(
                expected.OrderBy(v => v),
                Is.EquivalentTo(list.OrderBy(v => v)));

            expected.Clear();
            foreach (var v in dictionary.Values)
                expected.Add(v);

            Assert.That(
                expected.OrderBy(v => v),
                Is.EquivalentTo(list.OrderBy(v => v)));

            Assert.That(
                dictionary.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key),
                Is.EquivalentTo(list.OrderBy(v => v)));

            Assert.That(
                dictionary.Keys.OrderBy(v => v),
                Is.EquivalentTo(list.OrderBy(v => v)));

            Assert.That(
                dictionary.Values.OrderBy(v => v),
                Is.EquivalentTo(list.OrderBy(v => v)));

            foreach (var v in list)
            {
                Assert.That(dictionary.Keys.Contains(v), Is.True);
                Assert.That(dictionary.Values.Contains(v), Is.True);
                Assert.That(dictionary.ContainsKey(v), Is.True);

                Assert.That(((IDictionary<string, string>)dictionary).Keys.Contains(v), Is.True);
                Assert.That(((IDictionary<string, string>)dictionary).Values.Contains(v), Is.True);
                Assert.That(((IDictionary<string, string>)dictionary).ContainsKey(v), Is.True);

                Assert.That(((IDictionary)dictionary).Contains(v), Is.True);
            }
        }
    }

    #if NET9_0_OR_GREATER

    [Test]
    public void AlternateLookup()
    {
        var list = Enumerable
            .Range(0, 50)
            .Select(_ => Guid.NewGuid().ToString())
            .Select(v => new KeyValuePair<string, string>(v, v))
            .ToList();

        var lookup = new DictionarySlim<string, string>(list, StringComparer.OrdinalIgnoreCase)
            .GetAlternateLookup<ReadOnlySpan<char>>();

        foreach (var (key, value) in list)
        {
            Assert.That(lookup[key.AsSpan()], Is.EqualTo(value));
            Assert.That(lookup.ContainsKey(key.AsSpan()), Is.True);
            Assert.That(lookup.TryGetValue(key.AsSpan(), out var value1), Is.True);
            Assert.That(lookup.TryGetValue(key.AsSpan(), out var actualKey, out var value2), Is.True);

            Assert.That(actualKey, Is.EqualTo(key));

            Assert.That(value1, Is.EqualTo(value));
            Assert.That(value2, Is.EqualTo(value));
        }

        foreach (var (key, value) in list)
        {
            Assert.That(lookup.Remove(key.AsSpan(), out var actualKey, out var value1), Is.True);
            Assert.That(actualKey, Is.EqualTo(key));
            Assert.That(value1, Is.EqualTo(value));
        }

        Assert.That(lookup.Dictionary.Count, Is.EqualTo(0));

        foreach (var (key, value) in list)
            lookup[key.AsSpan()] = value;

        foreach (var (key, value) in list)
        {
            Assert.That(lookup[key.AsSpan()], Is.EqualTo(value));
            Assert.That(lookup.ContainsKey(key.AsSpan()), Is.True);
            Assert.That(lookup.TryGetValue(key.AsSpan(), out var value1), Is.True);
            Assert.That(lookup.TryGetValue(key.AsSpan(), out var actualKey, out var value2), Is.True);

            Assert.That(actualKey, Is.EqualTo(key));

            Assert.That(value1, Is.EqualTo(value));
            Assert.That(value2, Is.EqualTo(value));
        }
    }

    #endif

    private class Int32Comparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) =>
            x == y;

        public int GetHashCode(int obj) =>
            obj;
    }
}
