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
}
