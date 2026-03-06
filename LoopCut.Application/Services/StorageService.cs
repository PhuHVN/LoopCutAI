using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using LoopCut.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly ILogger<StorageService> _logger;

        public StorageService(IConfiguration configuration, ILogger<StorageService> logger)
        {
            try
            {
                var jsonKey = configuration["GoogleStorage:CredentialsJson"];
                _bucketName = configuration["GoogleStorage:BucketName"]?.Trim();

                var credential = GoogleCredential.FromJson(jsonKey);
                _storageClient = StorageClient.Create(credential);
                _logger = logger;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var objectName = fileUrl;
            var bucketPrefix = $"https://storage.googleapis.com/{_bucketName}/";
            if (fileUrl.Contains(bucketPrefix))
            {
                objectName = fileUrl.Replace(bucketPrefix, "");
            }
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectName);
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError($"Error delete file: {ex.Message}");
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }
            var uniqueFileName = $"loopcutai/{Guid.NewGuid()}_{file.FileName}";
            using var stream = file.OpenReadStream();
            var dataObject = await _storageClient.UploadObjectAsync(_bucketName, uniqueFileName, file.ContentType, stream);
            return $"https://storage.googleapis.com/{_bucketName}/{uniqueFileName}";
        }
    }
}
