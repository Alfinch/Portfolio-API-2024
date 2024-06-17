using AlfieWoodland.Function.Binding;
using Microsoft.AspNetCore.Http;
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

        public ProjectsGet(ILogger<ProjectsGet> logger)
        {
            _logger = logger;
        }

        [Function("ProjectsGet")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "projects")] HttpRequestData req)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            string query = "SELECT ([Id], [Title], [Description], [Image], [StartDate]) FROM [Projects]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var results = new List<Project>();

                    while (await reader.ReadAsync())
                    {
                        var result = new Project
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            Image = reader.GetGuid(3),
                            StartDate = reader.GetDateTime(4)
                        };
                        results.Add(result);
                    }

                    return new OkObjectResult(results);
                }
            }
        }
    }
}
