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
    public class GetMovieDetail
    {
        private readonly ILogger<GetMovieDetail> _logger;
        private readonly CosmosClient _cosmosClient;

        public GetMovieDetail(ILogger<GetMovieDetail> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }

        [Function("GetMovieDetail")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            try
            {
                var databaseName = Environment.GetEnvironmentVariable("DatabaseName");
                var containerName = Environment.GetEnvironmentVariable("ContainerName");

                var container = _cosmosClient.GetContainer(databaseName, containerName);

                var filterId = req.Query["id"];
                var query = "SELECT * FROM c WHERE c.id = @id";
                var querydefinition = new QueryDefinition(query).WithParameter("@id", filterId);
                var result = container.GetItemQueryIterator<Movie>(querydefinition);
                var results = new List<Movie>();

                while (result.HasMoreResults)
                {
                    foreach (var item in await result.ReadNextAsync())
                        results.Add(item);
                }

                var responseMessage = req.CreateResponse(HttpStatusCode.OK);
                await responseMessage.WriteAsJsonAsync(results.FirstOrDefault());

                return responseMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na function GetMovieDetail - {ex}");
                throw;
            }
        }
    }
}
