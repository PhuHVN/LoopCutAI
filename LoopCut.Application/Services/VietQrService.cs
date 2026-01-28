using LoopCut.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class VietQRService : IVietQrService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string ApiUrl = "https://api.vietqr.io/v2/generate";

    public VietQRService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<VietQRResponse> GenerateQRCodeAsync(VietQRGenerateRequest request)
    {
        try
        {
            var clientId = _configuration["VietQR:ClientId"];
            var apiKey = _configuration["VietQR:ApiKey"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("VietQR credentials not configured");
            }
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                )
            };

            httpRequest.Headers.Add("x-client-id", clientId);
            httpRequest.Headers.Add("x-api-key", apiKey);

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"VietQR API Error: {response.StatusCode} - {responseContent}");
            }
            var result = JsonConvert.DeserializeObject<VietQRResponse>(responseContent);

            if (result.Code != "00")
            {
                throw new Exception($"VietQR Error: {result.Code} - {result.Desc}");
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to generate VietQR: {ex.Message}", ex);
        }
    }
}