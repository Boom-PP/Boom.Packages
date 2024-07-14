using FluentAssertions;
using Xunit.Abstractions;

namespace Boom.IdType.UnitTests;

public class IdTests(ITestOutputHelper testOutputHelper)
{
    [Fact]//(Skip = "Just use for generating some IDs when needed")]
    public void GenerateIds()
    {
        for (int i = 0; i < 10; i++)
            testOutputHelper.WriteLine(Id.NewId());
    }
    
    [Fact]
    public void CanImplicitlyConvertToString()
    {
        var id = Id.NewId();

        string stringId = id;

        stringId.Should().BeOfType<string>(stringId);
        stringId.Should().Be(id.ToString());
    }
}