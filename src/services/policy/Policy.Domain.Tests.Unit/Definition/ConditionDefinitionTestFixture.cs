using NOCO.Threvw.Policies.Definition;

namespace Policy.Domain.Tests.Unit;

[TestClass]
public class ConditionDefinitionTestFixture {

    [TestMethod]
    public void Given_ValidProperties_When_ConditionDefinitionIsCreated_Then_PropertiesShouldBeInitializedCorrectly() {
        // Arrange
        var expectedInputSource = InputSources.TransactionContext;
        var expectedAttributePath = "path/to/attribute";
        var expectedDataType = "string";
        var expectedRequired = true;

        // Act
        var conditionDefinition = new ConditionDefinition {
            InputSource = expectedInputSource,
            ContextAttributePath = expectedAttributePath,
            DataType = expectedDataType,
            Required = expectedRequired
        };

        // Assert
        Assert.AreEqual(expectedInputSource, conditionDefinition.InputSource);
        Assert.AreEqual(expectedAttributePath, conditionDefinition.ContextAttributePath);
        Assert.AreEqual(expectedDataType, conditionDefinition.DataType);
        Assert.AreEqual(expectedRequired, conditionDefinition.Required);
    }

}
