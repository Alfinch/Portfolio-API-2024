using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AlfieWoodland.Function.Helper
{
    public static class ManagedIdentityHelper
    {
        public static async Task GetManagedIdentityAsync(SqlConnection connection, ILogger logger)
        {
            var environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            if (environment == "Development")
            {
                logger.LogInformation("Managed Identity disabled in development environment");

                return;
            }

            logger.LogInformation("Managed Identity enabled");

            var credential = new ManagedIdentityCredential();

            logger.LogInformation("Managed Identity credential created");

            var tokenRequestContext = new TokenRequestContext(["https://database.windows.net/.default"]);

            logger.LogInformation("Token request context created");

            var accessToken = await credential.GetTokenAsync(tokenRequestContext);

            if (String.IsNullOrEmpty(accessToken.Token))
            {
                logger.LogError("Access token not acquired");
            }
            else
            {
                logger.LogInformation("Access token acquired: {0}", accessToken.Token);
            }

            connection.AccessToken = accessToken.Token;
        }
    }
}