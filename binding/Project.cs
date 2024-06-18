namespace AlfieWoodland.Function.Binding
{
    public class Project
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Guid Image { get; set; }
        public DateTime StartDate { get; set; }
        public required IEnumerable<Update> Updates { get; set; }
    }
}