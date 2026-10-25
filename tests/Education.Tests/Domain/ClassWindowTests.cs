using Education.Domain.Courses;
using Education.Domain.Practicals;

namespace Education.Tests.Domain;

/// <summary>Правила пары: окно времени, продление, досрочное завершение.</summary>
public sealed class ClassWindowTests
{
    private static readonly DateTimeOffset Start = new(2026, 10, 25, 9, 0, 0, TimeSpan.Zero);
    private static readonly Guid Teacher = Guid.NewGuid();

    private static ClassWindow NewWindow(int minutes = 90) =>
        ClassWindow.Start(Guid.NewGuid(), "  G-1 ", Start, minutes, Teacher);

    [Fact]
    public void Start_TrimsGroup_AndSetsEnd()
    {
        var window = NewWindow(90);

        Assert.Equal("G-1", window.GroupName);
        Assert.Equal(Start.AddMinutes(90), window.EndsAt);
        Assert.Null(window.ClosedAt);
    }

    [Theory]
    [InlineData(ClassWindow.MinDurationMinutes - 1)]
    [InlineData(ClassWindow.MaxDurationMinutes + 1)]
    public void Start_RejectsDurationOutOfRange(int minutes)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NewWindow(minutes));
    }

    [Fact]
    public void IsOpenAt_IncludesStart_ExcludesEnd()
    {
        var window = NewWindow(60);

        Assert.False(window.IsOpenAt(Start.AddSeconds(-1)));
        Assert.True(window.IsOpenAt(Start));
        Assert.True(window.IsOpenAt(Start.AddMinutes(59)));
        Assert.False(window.IsOpenAt(Start.AddMinutes(60)));
    }

    [Fact]
    public void Extend_MovesEnd_OnlyWhileOpen()
    {
        var window = NewWindow(60);

        window.Extend(15, Start.AddMinutes(30));

        Assert.Equal(Start.AddMinutes(75), window.EndsAt);
        Assert.Throws<InvalidOperationException>(() => window.Extend(15, Start.AddMinutes(80)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(ClassWindow.MaxExtendMinutes + 1)]
    public void Extend_RejectsMinutesOutOfRange(int minutes)
    {
        var window = NewWindow();

        Assert.Throws<ArgumentOutOfRangeException>(() => window.Extend(minutes, Start));
    }

    [Fact]
    public void Finish_ClosesEarly_AndIsIdempotent()
    {
        var window = NewWindow(90);
        var closedAt = Start.AddMinutes(20);

        window.Finish(closedAt, Teacher);
        window.Finish(Start.AddMinutes(40), Guid.NewGuid());

        Assert.Equal(closedAt, window.ClosedAt);
        Assert.Equal(Teacher, window.ClosedByUserId);
        Assert.Equal(closedAt, window.EffectiveEnd);
        Assert.False(window.IsOpenAt(closedAt));
    }

    [Fact]
    public void BindExternalModule_StoresAttemptsPerSession_AndRejectsZero()
    {
        var practical = new PracticalMaterial(Guid.NewGuid(), "p");

        practical.BindExternalModule(3, 45, attemptsPerSession: 5);

        Assert.Equal(5, practical.AttemptsPerSession);
        Assert.Throws<ArgumentOutOfRangeException>(() => practical.BindExternalModule(3, null, attemptsPerSession: 0));
    }
}
