using Education.Domain.Common;
using Education.Domain.Materials;
using Education.Domain.Practicals;
using Education.Domain.Tests;

namespace Education.Domain.Courses;

public sealed class Module : Entity
{
    public string Name { get; private set; } = String.Empty;
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public List<PracticalMaterial> PracticalMaterials { get; private set; } = [];
    public List<Question> Questions { get; private set; } = [];
    public List<TheoreticalMaterial> TheoreticalMaterials { get; private set; } = [];

    private Module()
    {
    }

    public Module(Guid courseId, string name)
    {
        CourseId = courseId;
        Name = name;
    }
}

