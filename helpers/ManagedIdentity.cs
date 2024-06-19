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

            var clientId = Environment.GetEnvironmentVariable("ManagedIdentityClientId");

            logger.LogInformation($"Managed Identity Client ID: {clientId}");

            var credential = new ManagedIdentityCredential(clientId: clientId);

            logger.LogInformation("Managed Identity credential created");

            var tokenRequestContext = new TokenRequestContext(["https://database.windows.net/.default"]);

            logger.LogInformation("Token request context created");

            var accessToken = await credential.GetTokenAsync(tokenRequestContext);

            logger.LogInformation("Access token acquired");

            connection.AccessToken = accessToken.Token;
        }
    }
}