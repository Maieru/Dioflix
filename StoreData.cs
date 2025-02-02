using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Dioflix
{
    public class StoreData
    {
        private readonly ILogger<StoreData> _logger;

        public StoreData(ILogger<StoreData> logger)
        {
            _logger = logger;
        }

        [Function("StoreData")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
                    return new BadRequestObjectResult("O cabecalho 'file-type' é obrigatório");

                var fileType = fileTypeHeader.ToString();
                var form = await req.ReadFormAsync();
                var file = form.Files["file"];

                if (file == null || file.Length == 0)
                    return new BadRequestObjectResult("O arquivo não foi enviado");

                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var containerName = fileType;

                var containerClient = new BlobContainerClient(connectionString, containerName);
                await containerClient.CreateIfNotExistsAsync();
                await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

                var blobName = file.FileName;
                var blob = containerClient.GetBlobClient(blobName);

                using var stream = file.OpenReadStream();
                await blob.UploadAsync(stream, true);

                _logger.LogInformation("Imagem salva com sucesso!");

                return new OkObjectResult(new
                {
                    Message = "Arquivo armazenado com sucesso!",
                    BlobURI = blob.Uri
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na function StoreData - {ex}");
                throw;
            }
        }
    }
}
