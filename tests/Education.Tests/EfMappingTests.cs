using Education.Domain.Courses;
using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests;

public class EfMappingTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public EfMappingTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public void DbContext_MapsKeyEntitiesToLegacyTablesAndColumns()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var model = context.Model;

        AssertColumn<User>(model, nameof(User.Id), "Users", "id");
        AssertColumn<User>(model, nameof(User.Password), "Users", "password");
        AssertColumn<User>(model, nameof(User.RoleId), "Users", "role_id");
        AssertColumn<Course>(model, nameof(Course.UserId), "Courses", "user_id");
        AssertColumn<Course>(model, nameof(Course.Name), "Courses", "c_name");
        AssertColumn<PracticalMaterial>(model, nameof(PracticalMaterial.PercentForFive), "PracticalMaterials", "percent_for_five");
        AssertColumn<CaseFile>(model, nameof(CaseFile.UserId), "CaseFiles", "user_id");
        AssertColumn<TestResult>(model, nameof(TestResult.UserId), "TestResults", "user_id");
        AssertColumn<Question>(model, nameof(Question.Options), "Questions", "question_body");
        AssertColumn<Answer>(model, nameof(Answer.Answers), "Answers", "answer");
    }

    [Fact]
    public void UserRelations_KeepForeignKeysToLegacyUsersId()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var model = context.Model;

        AssertForeignKeyToUser<Course>(model, nameof(Course.UserId));
        AssertForeignKeyToUser<CourseBindUser>(model, nameof(CourseBindUser.UserId));
        AssertForeignKeyToUser<PracticalBindUser>(model, nameof(PracticalBindUser.UserId));
        AssertForeignKeyToUser<CaseFile>(model, nameof(CaseFile.UserId));
        AssertForeignKeyToUser<TestResult>(model, nameof(TestResult.UserId));
    }

    private static void AssertColumn<TEntity>(IModel model, string propertyName, string tableName, string columnName)
    {
        var entity = model.FindEntityType(typeof(TEntity)) ?? throw new InvalidOperationException();
        var storeObject = StoreObjectIdentifier.Table(tableName, null);
        var property = entity.FindProperty(propertyName) ?? throw new InvalidOperationException();

        Assert.Equal(tableName, entity.GetTableName());
        Assert.Equal(columnName, property.GetColumnName(storeObject));
    }

    private static void AssertForeignKeyToUser<TEntity>(IModel model, string propertyName)
    {
        var entity = model.FindEntityType(typeof(TEntity)) ?? throw new InvalidOperationException();
        var property = entity.FindProperty(propertyName) ?? throw new InvalidOperationException();
        var foreignKey = entity.GetForeignKeys().Single(fk => fk.Properties.Contains(property));
        var principalKeyProperty = foreignKey.PrincipalKey.Properties.Single();

        Assert.Equal(typeof(User), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal(nameof(User.Id), principalKeyProperty.Name);
    }
}
