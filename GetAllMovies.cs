using Dioflix.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Dioflix
{
    public class GetAllMovies
    {
        private readonly ILogger<GetAllMovies> _logger;
        private readonly CosmosClient _cosmosClient;

        public GetAllMovies(ILogger<GetAllMovies> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("GetAllMovies")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            try
            {
                var databaseName = Environment.GetEnvironmentVariable("DatabaseName");
                var containerName = Environment.GetEnvironmentVariable("ContainerName");

                var container = _cosmosClient.GetContainer(databaseName, containerName);

                var query = "SELECT * FROM c";
                var querydefinition = new QueryDefinition(query);
                var result = container.GetItemQueryIterator<Movie>(querydefinition);
                var results = new List<Movie>();

                while (result.HasMoreResults)
                {
                    foreach (var item in await result.ReadNextAsync())
                        results.Add(item);
                }

                var responseMessage = req.CreateResponse(HttpStatusCode.OK);
                await responseMessage.WriteAsJsonAsync(results);

                return responseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na function GetAllMovies - {ex}");
                throw;
            }
        }
    }
}
