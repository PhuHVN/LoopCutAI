using LoopCut.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Application.Services
{
    public class KeyService : IKeyService
    {
        private List<string> _apiKeys = new List<string>();
        private readonly IConfiguration _config;
        private readonly HashSet<string> _usedKeys = new HashSet<string>();
        private readonly IHttpClientFactory _httpClientFactory;
        private DateTime _lastRefreshTime = DateTime.MinValue;
        private int _currentIndex = 0;

        public KeyService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task RefreshApiKeysAsync()
        {
            // Refresh keys if we don't have any or if it's been more than 5 minutes since the last refresh
            if (_apiKeys.Any() && (DateTime.Now - _lastRefreshTime).TotalMinutes < 5) return;
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var csvData = await client.GetStringAsync(_config["GGSheet:Url"]!);
                var newKeys = csvData.Split('\n', '\r')
                    .Skip(1) // Skip header line
                    .Select(line => line.Split(','))
                    .Where(cols => cols.Length >= 2 && cols[1].Trim().Equals("Active", StringComparison.OrdinalIgnoreCase))
                    .Select(cols => cols[0].Trim())
                    .ToList();
                if (newKeys.Any())
                {
                    _apiKeys = newKeys;
                    _lastRefreshTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                if (!_apiKeys.Any()) throw new Exception("Failed to refresh API keys and no keys are currently available.", ex);
            }
        }

        public async Task<string> GetApiKeyAsync()
        {
            await RefreshApiKeysAsync();
            for (int i = 0; i < _apiKeys.Count; i++)
            {
                var key = _apiKeys[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _apiKeys.Count; // Move to next key for next call
                if (!_usedKeys.Contains(key))
                {
                    return key;
                }
            }
            _usedKeys.Clear(); // All keys have been used, reset the used keys set
            return _apiKeys[0];
        }
        public void ReportUsedKey(string key)
        {
            _usedKeys.Add(key);
        }
    }
}
