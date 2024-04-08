using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace gh_api_token_provider
{
    public class TokenRequest(ILoggerFactory loggerFactory)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<TokenRequest>();
        private HttpResponseData _response = null!;

        [Function("TokenRequest")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            string? secretName = Environment.GetEnvironmentVariable("SECRET_NAME", EnvironmentVariableTarget.Process);
            string? keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI", EnvironmentVariableTarget.Process);
            string? managedIdentityId = Environment.GetEnvironmentVariable("MANAGED_IDENTITY_ID", EnvironmentVariableTarget.Process);

            if (secretName is null || keyVaultUri is null)
            {
                _logger.LogError("Could not load settings for SecretName:[{secretName}], KeyVaultUri:[{keyVaultUri}]", secretName, keyVaultUri);
                _response = req.CreateResponse(HttpStatusCode.FailedDependency);
                _response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                _response.WriteString("Could not load settings");
                return _response;
            }
            else
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                // When deployed to an azure host, the default azure credential will authenticate the specified user assigned managed identity.

                var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityId });

                SecretClient client = new(new Uri(keyVaultUri!), credential);

                var secret = client.GetSecret(secretName);

                byte[] decodedData = Convert.FromBase64String(secret.Value.Value);
                string decodedString = System.Text.Encoding.UTF8.GetString(decodedData);

                _response = req.CreateResponse(HttpStatusCode.OK);
                _response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                _response.WriteString(decodedString);

                return _response;
            }
        }
    }
}
