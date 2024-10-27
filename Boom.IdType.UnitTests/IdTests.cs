using System.Text.Json;
using FluentAssertions;

using IdGen;

using Xunit.Abstractions;

namespace Boom.IdType.UnitTests;

public class IdTests(ITestOutputHelper testOutputHelper)
{
    [Fact] //(Skip = "Just use for generating some IDs when needed")]
    public void GenerateIds()
    {
        for (int i = 0; i < 10; i++)
            testOutputHelper.WriteLine(Id.NewId());
    }

    [Fact]
    public void KeepsFormatForLongTime()
    {
        // 80 years should suffice
        var timeSource = new DefaultTimeSource(IdGeneratorOptions.Default.TimeSource.Epoch + TimeSpan.FromDays(365 * 10) - TimeSpan.FromDays(365 * 80));
        var idOptions = new IdGeneratorOptions(timeSource: timeSource);
        Id.IdGeneratorInstance = new IdGenerator(1023, idOptions);
        
        testOutputHelper.WriteLine($"Current ticks   : {timeSource.GetTicks(),10}");
        testOutputHelper.WriteLine($"TimeSource ticks: {IdGeneratorOptions.Default.TimeSource.GetTicks(),10}");
        
        var id = Id.NewId();
        testOutputHelper.WriteLine($"Id from {timeSource}: {id}");
        
        id.ToString().Length.Should().Be(14);
        id.ToString()[4].Should().Be('-');
        id.ToString()[9].Should().Be('-');
    }

    [Fact]
    public void CanImplicitlyConvertToString()
    {
        var id = Id.NewId();

        string stringId = id;

        stringId.Should().BeOfType<string>(stringId);
        stringId.Should().Be(id.ToString());
    }
    
    [Fact]
    public void CanImplicitlyConvertNullToString()
    {
        Id? id = null;

        string stringId = id;

        stringId.Should().BeOfType<string>(stringId);
        stringId.Should().Be(string.Empty);
    }

    [Fact]
    public void IdShouldBeAwesomelyFormatted()
    {
        var id = new Id("8jywwL8S1hh");

        string formatted = id;

        formatted.Should().Be("8jyw-wL8S-1hh");
    }

    [Fact]
    public void IdFromExistingFormattedShouldBeAwesomelyFormatted()
    {
        var id = Id.FromExisting("8jyw-wL8S-1hh");

        string formatted = id;

        formatted.Should().Be("8jyw-wL8S-1hh");
    }

    [Fact]
    public void ShouldHandleJsonSerialization()
    {
        var id = Id.FromExisting("8jyw-wL8S-1hh");

        var serialized = JsonSerializer.Serialize(id);

        serialized.Should().Be("\"8jyw-wL8S-1hh\"");
    }

    [Fact]
    public void ShouldHandleJsonDeserialization()
    {
        var serialized = "\"8jyw-wL8S-1hh\"";

        var deserialized = JsonSerializer.Deserialize<Id>(serialized);

        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<Id>();
        deserialized!.ToString().Should().Be("8jyw-wL8S-1hh");
    }

    [Fact]
    public void ShouldSupportCreatingEmpty()
    {
        var emptyId = Id.Empty;
        
        emptyId.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void NoExternalShouldBeAllowedToInstantiateEmptyId()
    {
        var act = () => new Id(string.Empty);
        
        act.Should().Throw<ArgumentException>().WithMessage("Id must have a value");
    }

    [Fact]
    public void ShouldNotDieOnToStringOnEmptyId()
    {
        var id = Id.Empty;

        id.ToString().Should().BeEmpty();
    }
    
    [Fact]
    public void CanCompareIds()
    {
        var id1 = Id.NewId();
        var id2 = Id.NewId();

        (id1.CompareTo(id2) < 0).Should().BeTrue();
        (id2.CompareTo(id1) > 0).Should().BeTrue();
        (id1.CompareTo(id1) == 0).Should().BeTrue();
    }
    
    [Fact]
    public void CanCompareIdsWithOperators()
    {
        var id1 = Id.NewId();
        var id2 = Id.NewId();

        (id1 > id2).Should().BeFalse();
        (id1 < id2).Should().BeTrue();
        (id1 <= id2).Should().BeTrue();
        (id1 >= id2).Should().BeFalse();
    }
}