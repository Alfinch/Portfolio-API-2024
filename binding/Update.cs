namespace AlfieWoodland.Function.Binding
{
    public class UpdateSummary
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime Date { get; set; }
    }

    public class Update : UpdateSummary
    {
        public required string Body { get; set; }
    }
}