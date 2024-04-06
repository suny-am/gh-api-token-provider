using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace gh_api_token_provider
{
    public class TokenRequest
    {
        private readonly ILogger _logger;

        public TokenRequest(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TokenRequest>();
        }

        [Function("TokenRequest")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {

            HttpResponseData response;

            bool isLocalDevelopment = Convert.ToBoolean(Environment.GetEnvironmentVariable("IsLocalDevelopment") ?? "false");

            if (isLocalDevelopment)
            {
                response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                response.WriteString("Local development");

                return response;
            }

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string keyVaultName = "gh-api-tokens";
            string secretName = "GhApiToken";
            string keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";

            SecretClient client = new(new Uri(keyVaultUri), new DefaultAzureCredential());

            var secret = client.GetSecret(secretName);

            byte[] decodedData = Convert.FromBase64String(secret.Value.Value);
            string decodedString = System.Text.Encoding.UTF8.GetString(decodedData);

            response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString(decodedString);

            return response;
        }
    }
}
