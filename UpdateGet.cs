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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "update/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var tableClient = _tableServiceClient.GetTableClient(tableName: "Update");

            try
            {
                var entities = tableClient.QueryAsync<UpdateEntity>(filter: $"RowKey eq '{id}'");

                await foreach (var entity in entities)
                {
                    var update = new Update
                    {
                        Id = new Guid(entity.RowKey),
                        Slug = entity.Slug,
                        Title = entity.Title,
                        Body = entity.Body,
                        Date = entity.Date
                    };

                    return new OkObjectResult(update);
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
