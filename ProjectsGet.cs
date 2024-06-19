using AlfieWoodland.Function.Binding;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "projects")] HttpRequestData req)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            string query = "SELECT p.[Id], p.[Title], p.[Description], p.[Image], p.[StartDate], u.[Id] as [UpdateId], u.[Title] as [UpdateTitle], u.[Date] as [UpdateDate] FROM [Project] p LEFT JOIN [UPDATE] u ON p.[Id] = u.[ProjectId]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var projects = new List<Project<UpdateSummary>>();

                    if (!await reader.ReadAsync())
                    {
                        return new OkObjectResult(projects);
                    }

                    do
                    {
                        var updates = new List<UpdateSummary>();

                        // If the project exists, create a new project object
                        var project = new Project<UpdateSummary>
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            Image = reader.GetGuid(3),
                            StartDate = reader.GetDateTime(4),
                            Updates = updates
                        };

                        projects.Add(project);

                        // If the first update ID is null there are no updates
                        if (reader.IsDBNull(4))
                        {
                            return new OkObjectResult(project);
                        }

                        // If the project has updates, add them to the project
                        do
                        {
                            var update = new UpdateSummary
                            {
                                Id = reader.GetInt32(5),
                                Title = reader.GetString(6),
                                Date = reader.GetDateTime(7)
                            };

                            updates.Add(update);

                        } while (await reader.ReadAsync() && projects.Last().Id == reader.GetInt32(0));

                    } while (await reader.ReadAsync());

                    return new OkObjectResult(projects);
                }
            }
        }
    }
}
