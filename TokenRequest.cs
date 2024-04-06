using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
