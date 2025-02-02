using Dioflix.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dioflix
{
    public class StoreDatabase
    {
        private readonly ILogger<StoreDatabase> _logger;

        public StoreDatabase(ILogger<StoreDatabase> logger)
        {
            _logger = logger;
        }

        [Function("StoreDatabase")]
        [CosmosDBOutput("%DatabaseName%", "%ContainerName%", Connection = "CosmosDbConnection", CreateIfNotExists = true, PartitionKey = "/id")]
        public async Task<string> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var content = await new StreamReader(req.Body).ReadToEndAsync();
                var movieRequest = JsonConvert.DeserializeObject<Movie>(content);

                return JsonConvert.SerializeObject(movieRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na function StoreData - {ex}");
                throw;
            }
        }
    }
}
