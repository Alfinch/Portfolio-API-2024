using Azure;
using Azure.Data.Tables;

namespace AlfieWoodland.Function.Entity
{
    public class UpdateEntity : ITableEntity
    {
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public required string Title { get; set; }
        public required string Body { get; set; }
        public DateTime Date { get; set; }
    }
}