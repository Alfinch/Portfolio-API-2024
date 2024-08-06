using Azure;
using Azure.Data.Tables;

namespace AlfieWoodland.Function.Entity
{
    public class ProjectEntity : ITableEntity
    {
        public required string PartitionKey { get; set; }
        public required string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public required string Title { get; set; }
        public required string Description { get; set; }
        public Guid Image { get; set; }
        public required string Slug { get; set; }
    }
}