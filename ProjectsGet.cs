using AlfieWoodland.Function.Entity;
using AlfieWoodland.Function.Model;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function
{
    public class ProjectsGet
    {
        private readonly ILogger<ProjectsGet> _logger;
        private readonly TableServiceClient _tableServiceClient;

        public ProjectsGet(ILogger<ProjectsGet> logger)
        {
            _logger = logger;
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString") ?? throw new ArgumentNullException("AzureStorageConnectionString");
            _tableServiceClient = new TableServiceClient(storageConnectionString);
        }

        [Function("ProjectsGet")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "projects")] HttpRequestData req)
        {
            _logger.LogInformation("Projects GET function processed a request.");

            var projectTableClient = _tableServiceClient.GetTableClient(tableName: "Project");
            var updateTableClient = _tableServiceClient.GetTableClient(tableName: "Update");

            try
            {
                var projectEntities = projectTableClient.QueryAsync<ProjectEntity>();

                var projects = new List<Project<UpdateSummary>>();

                await foreach (var projectEntity in projectEntities)
                {
                    var updateEntities = updateTableClient.QueryAsync<UpdateEntity>(filter: $"PartitionKey eq '{projectEntity.RowKey}'");

                    var updates = new List<UpdateSummary>();

                    await foreach (var updateEntity in updateEntities)
                    {
                        var update = new UpdateSummary
                        {
                            Id = new Guid(updateEntity.RowKey),
                            Title = updateEntity.Title,
                            Date = updateEntity.Date
                        };

                        updates.Add(update);
                    }

                    updates = updates.OrderBy(u => u.Date).ToList();

                    var project = new Project<UpdateSummary>
                    {
                        Id = new Guid(projectEntity.RowKey),
                        Title = projectEntity.Title,
                        Description = projectEntity.Description,
                        Image = projectEntity.Image,
                        FirstUpdated = updates.Any() ? updates.Min(u => u.Date) : null,
                        LastUpdated = updates.Any() ? updates.Max(u => u.Date) : null,
                        Updates = updates
                    };

                    projects.Add(project);
                }

                projects = projects.OrderByDescending(p => p.LastUpdated).ToList();

                return new OkObjectResult(projects);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return new NotFoundResult();
            }
        }
    }
}
