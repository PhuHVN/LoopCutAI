using LoopCut.Application.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace LoopCut.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IDatabase? _redis;
        private readonly ILogger<OtpService> _logger;
        private readonly ConcurrentDictionary<string, (string, DateTime)> _memoryCache = new();
        private bool _useMemoryFallback;

        public OtpService(IConnectionMultiplexer? redis, ILogger<OtpService> logger)
        {
            _logger = logger;
            try
            {
                _redis = redis?.GetDatabase();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis. Falling back to in-memory cache.");
                _useMemoryFallback = true;
            }
            _useMemoryFallback = _redis == null;
        }

        public string GenerateOTP()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }

        public async Task RemoveOtpAsync(string key)
        {
            if (_useMemoryFallback)
            {
                _memoryCache.TryRemove(key, out _);
                return;
            }
            try
            {
                await _redis!.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove OTP from Redis. Falling back to in-memory cache.");
                _useMemoryFallback = true;
                _memoryCache.TryRemove(key, out _);
                return;
            }
        }

        public async Task<string?> RetrieveOtpAsync(string key)
        {
            if (_useMemoryFallback)
            {
                if (_memoryCache.TryGetValue(key, out var value))
                {
                    if (value.Item2 > DateTime.UtcNow)
                    {
                        return value.Item1;
                    }
                    else
                    {
                        _memoryCache.TryRemove(key, out _);
                        return null;
                    }
                }
                return null;
            }
            try
            {
                return await _redis!.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve OTP from Redis. Falling back to in-memory cache.");
                _useMemoryFallback = true;
                return null;
            }
        }

        public async Task StoreOtpAsync(string key, string otp, TimeSpan expiration)
        {
            if (_useMemoryFallback)
            {
                var expiryTime = DateTime.UtcNow.Add(expiration);
                _memoryCache[key] = (otp, expiryTime);
                return;
            }
            try
            {
                await _redis!.StringSetAsync(key, otp, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store OTP in Redis. Falling back to in-memory cache.");
                _useMemoryFallback = true;
                var expiryTime = DateTime.UtcNow.Add(expiration);
                _memoryCache[key] = (otp, expiryTime);
                return;
            }
        }
    }
}
