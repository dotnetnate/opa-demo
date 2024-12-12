public interface IDataGenerationStrategy {
    string PolicyName { get; }
    void GenerateSampleData(int numberOfResources, int numberOfSubjectsPerResource, int numberOfPermissionsPerSubject, string outputFolder);
}