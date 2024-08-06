using AlfieWoodland.Function.Entity;
using AlfieWoodland.Function.Model;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function
{
    public class UpdateGet
    {
        private readonly ILogger<UpdateGet> _logger;
        private readonly TableServiceClient _tableServiceClient;

        public UpdateGet(ILogger<UpdateGet> logger)
        {
            _logger = logger;
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString") ?? throw new ArgumentNullException("AzureStorageConnectionString");
            _tableServiceClient = new TableServiceClient(storageConnectionString);
        }

        [Function("UpdateGet")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project/{projectSlug}/{updateSlug}")] HttpRequest req, string projectSlug, string updateSlug)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var tableClient = _tableServiceClient.GetTableClient(tableName: "Update");

            try
            {
                var projectEntities = tableClient.QueryAsync<UpdateEntity>(filter: $"Slug eq '{projectSlug}'");

                await foreach (var projectEntity in projectEntities)
                {
                    var updateEntities = tableClient.QueryAsync<UpdateEntity>(filter: $"PartitionKey eq '{projectEntity.RowKey}' and Slug eq '{updateSlug}'");

                    await foreach (var updateEntity in updateEntities)
                    {
                        var update = new Update
                        {
                            Slug = updateEntity.Slug,
                            Title = updateEntity.Title,
                            Body = updateEntity.Body,
                            Date = updateEntity.Date
                        };

                        return new OkObjectResult(update);
                    }

                    return new NotFoundResult();
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
