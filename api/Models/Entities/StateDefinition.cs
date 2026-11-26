namespace Inspections.Models.Entities;

public class StateDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string PlaceIdsJson { get; set; } = "[]";
}
