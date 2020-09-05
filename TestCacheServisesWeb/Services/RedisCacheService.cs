using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestCacheServisesWeb.Controllers;
using TestCacheServisesWeb.DistributeCaching;

namespace TestCacheServisesWeb.Services
{
    public class RedisCacheService
    {
        private IDistributedCache _distributedCache;
        private readonly ILogger<HomeController> _logger;
        public RedisCacheService(IDistributedCache distributedCache, ILogger<HomeController> logger)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<List<Dictionary<string, object>>> GetOrCreateOrdersListWaitAndPolicy(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            List<Dictionary<string, object>> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();

                try
                {
                    encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                        await _distributedCache.SetAsync(key, encodedData, options);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(serializedData);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<Dictionary<string, object>> GetOrCreateOrderWaitAndPolicy(Func<Task<Dictionary<string, object>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            Dictionary<string, object> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();

                try
                {
                    encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                        await _distributedCache.SetAsync(key, encodedData, options);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<List<string>> GetOrCreateCitiesWaitAndPolicy(Func<Task<List<string>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            List<string> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();

                try
                {
                    encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                        await _distributedCache.SetAsync(key, encodedData, options);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<List<string>>(serializedData);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<string>>(serializedData);
            }
            return cacheEntry;
        }
        // simple cache
        public async Task<List<string>> GetOrCreateCitiesSimple(Func<Task<List<string>>> query, string key)
        {
            List<string> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                cacheEntry = await query();
                serializedData = JsonConvert.SerializeObject(cacheEntry);
                encodedData = Encoding.UTF8.GetBytes(serializedData);
                await _distributedCache.SetAsync(key, encodedData);
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<string>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<List<Dictionary<string, object>>> GetOrCreateOrdersSimple(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
            List<Dictionary<string, object>> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                cacheEntry = await query();
                serializedData = JsonConvert.SerializeObject(cacheEntry);
                encodedData = Encoding.UTF8.GetBytes(serializedData);
                await _distributedCache.SetAsync(key, encodedData);
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<Dictionary<string, object>> GetOrCreateOrderSimple(Func<Task<Dictionary<string, object>>> query, string key)
        {
            Dictionary<string, object> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
           // _logger.LogInformation(Encoding.UTF8.GetString(encodedData));
            if (encodedData == null)
            { 
                _logger.LogInformation("NOT IN CACHE, CREATED ");
                cacheEntry = await query();
                serializedData = JsonConvert.SerializeObject(cacheEntry);
                encodedData = Encoding.UTF8.GetBytes(serializedData);
                await _distributedCache.SetAsync(key, encodedData);
            }
            else
            {
                _logger.LogInformation("GET FROM  CACHE ");
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
            }
            _logger.LogInformation("END FUNCTION ");
            return cacheEntry;
        }
        // wait cache
        public async Task<List<Dictionary<string, object>>> GetOrCreateOrdersWait(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            List<Dictionary<string, object>> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();

                try
                {
                    encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        await _distributedCache.SetAsync(key, encodedData);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(serializedData);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<Dictionary<string, object>> GetOrCreateOrderWait(Func<Task<Dictionary<string, object>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            Dictionary<string, object> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();

                try
                {
                    encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        await _distributedCache.SetAsync(key, encodedData);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<List<string>> GetOrCreateCitiesWait(Func<Task<List<string>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            List<string> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();

                try
                {
                    encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        await _distributedCache.SetAsync(key, encodedData);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<List<string>>(serializedData);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<string>>(serializedData);
            }
            return cacheEntry;
        }

        // policy
        public async Task<List<Dictionary<string, object>>> GetOrCreateOrdersPolicy(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
            List<Dictionary<string, object>> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
                    if (encodedData == null)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        serializedData = JsonConvert.SerializeObject(cacheEntry);
                        encodedData = Encoding.UTF8.GetBytes(serializedData);
                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                        await _distributedCache.SetAsync(key, encodedData, options);
                    }
                    else
                    {
                        serializedData = Encoding.UTF8.GetString(encodedData);
                        cacheEntry = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(serializedData);
                    }
            return cacheEntry;
        }

        public async Task<Dictionary<string, object>> GetOrCreateOrderPolicy(Func<Task<Dictionary<string, object>>> query, string key)
        {
            Dictionary<string, object> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)// Key not in cache, so get data.
            {
                cacheEntry = await query();
                serializedData = JsonConvert.SerializeObject(cacheEntry);
                encodedData = Encoding.UTF8.GetBytes(serializedData);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                await _distributedCache.SetAsync(key, encodedData, options);
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
            }
            return cacheEntry;
        }

        public async Task<List<string>> GetOrCreateCitiesPolicy(Func<Task<List<string>>> query, string key)
        {
            List<string> cacheEntry;
            string serializedData;
            var encodedData = await _distributedCache.GetAsync(key);
            if (encodedData == null)// Key not in cache, so get data.
            {
                cacheEntry = await query();
                serializedData = JsonConvert.SerializeObject(cacheEntry);
                encodedData = Encoding.UTF8.GetBytes(serializedData);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(1));
                await _distributedCache.SetAsync(key, encodedData, options);
            }
            else
            {
                serializedData = Encoding.UTF8.GetString(encodedData);
                cacheEntry = JsonConvert.DeserializeObject<List<string>>(serializedData);
            }
            return cacheEntry;
        }

        public void cleanRedisCache()
        {
             _distributedCache.Remove(CacheKeys.OrderInfoRedis);
             _distributedCache.Remove(CacheKeys.OrdersInfoRedis);
             _distributedCache.Remove(CacheKeys.CityListRedis);

            _distributedCache.Remove(CacheKeys.OrderInfoPolicyRedis);
            _distributedCache.Remove(CacheKeys.OrdersInfoPolicyRedis);
            _distributedCache.Remove(CacheKeys.CityListPolicyRedis);

            _distributedCache.Remove(CacheKeys.OrderInfoWaitRedis);
            _distributedCache.Remove(CacheKeys.OrdersInfoWaitRedis);
            _distributedCache.Remove(CacheKeys.CityListWaitRedis);

            _distributedCache.Remove(CacheKeys.OrderInfoWaitAndPolicyRedis);
            _distributedCache.Remove(CacheKeys.OrdersInfoWaitAndPolicyRedis);
            _distributedCache.Remove(CacheKeys.CityListWaitAndPolicyRedis);
        }
    }
}
