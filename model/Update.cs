namespace AlfieWoodland.Function.Model
{
    public class UpdateSummary
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public DateTime Date { get; set; }
    }

    public class Update : UpdateSummary
    {
        public required string Body { get; set; }
    }
}