using TableCreatorTool;

try
{
    var creator = new TableCreator();
    creator.CreateTables("config.json");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
Console.WriteLine("Press Any key to exit.");
Console.Read();