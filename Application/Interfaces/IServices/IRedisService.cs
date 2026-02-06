using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service interface for Redis caching operations (Placeholder).
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Stores a value in Redis with an expiration time.
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan expiration);

        /// <summary>
        /// Retrieves a value from Redis by key.
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Removes a value from Redis by key.
        /// </summary>
        Task RemoveAsync(string key);
    }
}