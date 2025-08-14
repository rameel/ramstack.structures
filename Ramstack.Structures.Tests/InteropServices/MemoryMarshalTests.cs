namespace Ramstack.InteropServices;

[TestFixture]
public class MemoryMarshalTests
{
    [Test]
    public void CreateStringView()
    {
        Assert.That(
            MemoryMarshal.CreateStringView("Hello, World!", 7).ToString(),
            Is.EqualTo("World!"));

        Assert.That(
            MemoryMarshal.CreateStringView("Hello, World!", 7).Length,
            Is.EqualTo(6));

        Assert.That(
            MemoryMarshal.CreateStringView("Hello, World!", 0, 5).ToString(),
            Is.EqualTo("Hello"));

        Assert.That(
            MemoryMarshal.CreateStringView("Hello, World!", 0, 5).Length,
            Is.EqualTo(5));
    }

    [Test]
    public void CreateArrayView()
    {
        var array = "Hello, World!".ToArray();

        Assert.That(
            MemoryMarshal.CreateArrayView(array, 7).AsSpan().ToString(),
            Is.EqualTo("World!"));

        Assert.That(
            MemoryMarshal.CreateArrayView(array, 7).Length,
            Is.EqualTo(6));

        Assert.That(
            MemoryMarshal.CreateArrayView(array, 0, 5).AsSpan().ToString(),
            Is.EqualTo("Hello"));

        Assert.That(
            MemoryMarshal.CreateArrayView(array, 0, 5).Length,
            Is.EqualTo(5));
    }

    [Test]
    public static void ReadOnlyMemory_TryGetStringView()
    {
        const string Input = "0123456789";

        var memory = Input.AsMemory();
        Assert.That(memory.IsEmpty, Is.False);

        Assert.That(MemoryMarshal.TryGetStringView(memory, out var view), Is.True);
        Assert.That(view, Is.EqualTo(Input));

        memory = memory[1..];
        Assert.That(MemoryMarshal.TryGetStringView(memory, out view), Is.True);
        Assert.That(view, Is.EqualTo(Input[1..]));
        Assert.That(view.Length, Is.EqualTo(Input.Length - 1));

        memory = memory[1..];
        Assert.That(MemoryMarshal.TryGetStringView(memory, out view), Is.True);
        Assert.That(view, Is.EqualTo(Input[2..]));
        Assert.That(view.Length, Is.EqualTo(Input.Length - 2));

        memory = memory.Slice(3, 2);
        Assert.That(MemoryMarshal.TryGetStringView(memory, out view), Is.True);
        Assert.That(view, Is.EqualTo(Input[5..7]));
        Assert.That(view.Length, Is.EqualTo(2));

        memory = memory[memory.Length..];
        Assert.That(MemoryMarshal.TryGetStringView(memory, out view), Is.True);
        Assert.That(view.Length, Is.Zero);

        memory = memory[..];
        Assert.That(MemoryMarshal.TryGetStringView(memory, out view), Is.True);
        Assert.That(view.Length, Is.Zero);

        memory = memory[..0];
        Assert.That(MemoryMarshal.TryGetStringView(memory, out view), Is.True);
        Assert.That(view.Length, Is.Zero);

        Assert.That(memory.IsEmpty, Is.True);
    }

    [Test]
    public static void ReadOnlyMemory_TryGetStringView_Array_ReturnsFalse()
    {
        var memory = new char[10].AsMemory();
        Assert.That(MemoryMarshal.TryGetStringView(memory, out var view), Is.False);
        Assert.That(view.IsDefault, Is.True);
    }

    [Test]
    public static void ReadOnlyMemory_TryGetArrayView()
    {
        var buffer = Enumerable.Range(0, 10).ToArray();
        var memory = buffer.AsMemory();

        Assert.That(MemoryMarshal.TryGetArrayView<int>(memory, out var view), Is.True);
        Assert.That(view, Is.EquivalentTo(buffer));
    }

    [Test]
    public static void ReadOnlyMemory_TryGetArrayView_EmptyArray()
    {
        var buffer = Array.Empty<int>();
        var memory = buffer.AsMemory();

        Assert.That(MemoryMarshal.TryGetArrayView<int>(memory, out var view), Is.True);
        Assert.That(view, Is.EquivalentTo(buffer));
    }

    [Test]
    public static void ReadOnlyMemory_TryGetArrayView_Default()
    {
        var memory = default(ReadOnlyMemory<int>);

        Assert.That(MemoryMarshal.TryGetArrayView(memory, out var view), Is.True);
        Assert.That(view, Is.EquivalentTo(Array.Empty<int>()));
    }

    [Test]
    public static void ReadOnlyMemory_TryGetArrayView_Empty()
    {
        var memory = ReadOnlyMemory<int>.Empty;

        Assert.That(MemoryMarshal.TryGetArrayView(memory, out var view), Is.True);
        Assert.That(view, Is.EquivalentTo(Array.Empty<int>()));
    }

    [Test]
    public static void ReadOnlyMemory_TryGetArrayView_String_ReturnsFalse()
    {
        var memory = "0123456789".AsMemory();
        Assert.That(MemoryMarshal.TryGetArrayView(memory, out var view), Is.False);
        Assert.That(view.IsDefault, Is.True);
    }
}
