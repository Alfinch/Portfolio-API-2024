using AlfieWoodland.Function.Entity;
using AlfieWoodland.Function.Model;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function
{
    public class ProjectGet
    {
        private readonly ILogger<ProjectsGet> _logger;
        private readonly TableServiceClient _tableServiceClient;

        public ProjectGet(ILogger<ProjectsGet> logger)
        {
            _logger = logger;
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString") ?? throw new ArgumentNullException("AzureStorageConnectionString");
            _tableServiceClient = new TableServiceClient(storageConnectionString);
        }

        [Function("ProjectGet")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project/{slug}")] HttpRequestData req, string slug)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var projectTableClient = _tableServiceClient.GetTableClient(tableName: "Project");
            var updateTableClient = _tableServiceClient.GetTableClient(tableName: "Update");

            try
            {
                var projectEntities = projectTableClient.QueryAsync<ProjectEntity>(filter: $"Slug eq '{slug}'");

                await foreach (var projectEntity in projectEntities)
                {
                    var updateEntities = updateTableClient.QueryAsync<UpdateEntity>(filter: $"PartitionKey eq '{projectEntity.RowKey}'");

                    var updates = new List<Update>();

                    await foreach (var updateEntity in updateEntities)
                    {
                        var update = new Update
                        {
                            Id = new Guid(updateEntity.RowKey),
                            Slug = updateEntity.Slug,
                            Title = updateEntity.Title,
                            Body = updateEntity.Body,
                            Date = updateEntity.Date
                        };

                        updates.Add(update);
                    }

                    updates = updates.OrderBy(u => u.Date).ToList();

                    var project = new Project<Update>
                    {
                        Id = new Guid(projectEntity.RowKey),
                        Slug = projectEntity.Slug,
                        Title = projectEntity.Title,
                        Description = projectEntity.Description,
                        Image = projectEntity.Image,
                        FirstUpdated = updates.Any() ? updates.Min(u => u.Date) : null,
                        LastUpdated = updates.Any() ? updates.Max(u => u.Date) : null,
                        Updates = updates
                    };

                    return new OkObjectResult(project);
                }

                return new NotFoundResult();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return new NotFoundResult();
            }
        }
    }
}
