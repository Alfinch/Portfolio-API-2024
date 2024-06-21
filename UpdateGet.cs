using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function
{
    public class UpdateGet
    {
        private readonly ILogger<UpdateGet> _logger;

        public UpdateGet(ILogger<UpdateGet> logger)
        {
            _logger = logger;
        }

        [Function("UpdateGet")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "update/{id}")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
