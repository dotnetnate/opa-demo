using CitizensFinancialGroup.Threvw.Policies.Definition;

namespace Policy.Domain.Tests.Unit;

[TestClass]
public class PermissionDefinitionTestFixture {
    [TestMethod]
    public void Given_Valid_Properties_When_Permission_Definition_Is_Created_Then_Properties_Should_Be_Initialized_Correctly() {
        // Arrange
        var name = "PermissionName";
        var visibility = new Visibility { Enabled = true };
        var appliesTo = new List<PermissionTarget>
        {
                    new PermissionTarget { Scope = "Scope1", ObjectTypes = new List<string> { "Type1" } }
                };
        var allowedConditions = new List<ConditionDefinition>
        {
                    new ConditionDefinition { ContextAttributePath = "path/to/attribute", DataType = "string", Required = true }
                };

        // Act
        var permissionDefinition = new PermissionDefinition {
            Name = name,
            Visibility = visibility,
            AppliesTo = appliesTo,
            AllowedConditions = allowedConditions
        };

        // Assert
        Assert.AreEqual(name, permissionDefinition.Name);
        Assert.AreEqual(visibility, permissionDefinition.Visibility);
        CollectionAssert.AreEqual(appliesTo, permissionDefinition.AppliesTo);
        CollectionAssert.AreEqual(allowedConditions, permissionDefinition.AllowedConditions);
    }
}
