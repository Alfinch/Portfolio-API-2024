using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function
{
    public class ProjectCRUD
    {
        private readonly ILogger<ProjectCRUD> _logger;

        public ProjectCRUD(ILogger<ProjectCRUD> logger)
        {
            _logger = logger;
        }

        [Function("ProjectCRUD")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
