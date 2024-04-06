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
        public void Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {

            bool isLocalDevelopment = Convert.ToBoolean(Environment.GetEnvironmentVariable("IsLocalDevelopment") ?? "false");

            if (isLocalDevelopment) SendLocal(req);

            SendToken(req);
        }

        private HttpResponseData SendLocal(HttpRequestData req)
        {
            _response = req.CreateResponse(HttpStatusCode.OK);
            _response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            _response.WriteString("Local development");

            return _response;
        }

        private HttpResponseData SendToken(HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string keyVaultName = "gh-api-tokens";
            string secretName = "GhApiToken";
            string keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";

            SecretClient client = new(new Uri(keyVaultUri), new DefaultAzureCredential());

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
