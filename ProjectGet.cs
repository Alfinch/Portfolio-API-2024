using System.Net;
using AlfieWoodland.Function.Binding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function
{
    public class ProjectGet
    {
        private readonly ILogger<ProjectsGet> _logger;

        public ProjectGet(ILogger<ProjectsGet> logger)
        {
            _logger = logger;
        }

        [Function("ProjectGet")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "project/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            string query = "SELECT ([Title], [Description], [Image], [StartDate]) FROM [Project] WHERE [Id] = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var result = new Project
                        {
                            Id = id,
                            Title = reader.GetString(0),
                            Description = reader.GetString(1),
                            Image = reader.GetGuid(2),
                            StartDate = reader.GetDateTime(3)
                        };
                        return new OkObjectResult(result);
                    }
                    else
                    {
                        return new NotFoundResult();
                    }
                }
            }
        }
    }
}
