namespace AlfieWoodland.Function.Binding
{
    public class Project<T> where T : UpdateSummary
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Guid Image { get; set; }
        public DateTime StartDate { get; set; }
        public required IEnumerable<T> Updates { get; set; }
    }
}