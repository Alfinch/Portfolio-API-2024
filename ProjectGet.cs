using AlfieWoodland.Function.Binding;
using AlfieWoodland.Function.Helper;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "project/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            string query = "SELECT p.[Title], p.[Description], p.[Image], p.[StartDate], u.[Id] as [UpdateId], u.[Title] as [UpdateTitle], u.[Body] as [UpdateBody], u.[Date] as [UpdateDate] FROM [Project] p LEFT JOIN [UPDATE] u ON p.[Id] = u.[ProjectId] WHERE p.[Id] = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await ManagedIdentityHelper.GetManagedIdentityAsync(conn, _logger);

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    // If the project does not exist, return a 404
                    if (!await reader.ReadAsync())
                    {
                        return new NotFoundResult();
                    }

                    var updates = new List<Update>();

                    // If the project exists, create a new project object
                    var project = new Project<Update>
                    {
                        Id = id,
                        Title = reader.GetString(0),
                        Description = reader.GetString(1),
                        Image = reader.GetGuid(2),
                        StartDate = reader.GetDateTime(3),
                        Updates = updates
                    };

                    // If the first update ID is null there are no updates
                    if (reader.IsDBNull(4))
                    {
                        return new OkObjectResult(project);
                    }

                    // If the project has updates, add them to the project
                    do
                    {
                        var update = new Update
                        {
                            Id = reader.GetInt32(4),
                            Title = reader.GetString(5),
                            Body = reader.GetString(6),
                            Date = reader.GetDateTime(7)
                        };

                        updates.Add(update);

                    } while (await reader.ReadAsync());

                    return new OkObjectResult(project);
                }
            }
        }
    }
}
