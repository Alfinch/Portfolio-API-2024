namespace AlfieWoodland.Function.Model
{
    public class Project<T> where T : UpdateSummary
    {
        public Guid Id { get; set; }
        public required string Slug { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Guid Image { get; set; }
        public DateTime? FirstUpdated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public required IEnumerable<T> Updates { get; set; }
    }
}