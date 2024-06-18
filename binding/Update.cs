namespace AlfieWoodland.Function.Binding
{
    public class Update
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public DateTime Date { get; set; }
    }
}