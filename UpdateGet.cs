using AlfieWoodland.Function.Binding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "update/{id}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Project GET function processed a request.");

            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            string query = "SELECT [Title], [Body], [Date] FROM [UPDATE] WHERE [Id] = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    // If the update does not exist, return a 404
                    if (!await reader.ReadAsync())
                    {
                        return new NotFoundResult();
                    }

                    // If the update exists, create a new update object
                    var update = new Update
                    {
                        Id = id,
                        Title = reader.GetString(0),
                        Body = reader.GetString(1),
                        Date = reader.GetDateTime(2)
                    };

                    return new OkObjectResult(update);
                }
            }
        }
    }
}
