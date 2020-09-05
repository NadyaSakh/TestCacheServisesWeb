using Enyim.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestCacheServisesWeb.Controllers;
using TestCacheServisesWeb.DistributeCaching;

// todo как переписать без повторений функции кеширования для разных типов?
namespace TestCacheServisesWeb.Services
{
    // memcached можно воспользоавться и через IDistributedCache
    public class MemcacheCacheService
    {
        private readonly IMemcachedClient _memcachedClient;
        private readonly ILogger<HomeController> _logger;
        public MemcacheCacheService(IMemcachedClient memcachedClient, ILogger<HomeController> logger)
        {
            _memcachedClient = memcachedClient;
            _logger = logger;
        }

        public async Task<List<Dictionary<string, object>>> GetOrCreateOrdersListWaitAndPolicyMemcached(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            List<Dictionary<string, object>> cacheEntry;

            var result = await _memcachedClient.GetAsync<List<Dictionary<string, object>>>(key);
            if (!result.Success)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();
                try
                {
                    result = await _memcachedClient.GetAsync<List<Dictionary<string, object>>>(key);
                    if (!result.Success)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        await _memcachedClient.AddAsync(key, cacheEntry, 100);
                    }
                    else
                    {
                        cacheEntry = result.Value;
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                cacheEntry = result.Value;
            }
            return cacheEntry;
        }

        public async Task<Dictionary<string, object>> GetOrCreateOrderWaitAndPolicyMemcached(Func<Task<Dictionary<string, object>>> query,
            string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            Dictionary<string, object> cacheEntry;

            var result = await _memcachedClient.GetAsync<Dictionary<string, object>>(key);
            if (!result.Success)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();
                try
                {
                    result = await _memcachedClient.GetAsync<Dictionary<string, object>>(key);
                    if (!result.Success)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        await _memcachedClient.AddAsync(key, cacheEntry, 100);
                    }
                    else
                    {
                        cacheEntry = result.Value;
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                cacheEntry = result.Value;
            }
            return cacheEntry;
        }

        public async Task<List<string>> GetOrCreateCitiesWaitAndPolicyMemcached(Func<Task<List<string>>> query, string key)
        {
            ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
            List<string> cacheEntry;

            var result = await _memcachedClient.GetAsync<List<string>>(key);
            if (!result.Success)
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
                await mylock.WaitAsync();
                try
                {
                    result = await _memcachedClient.GetAsync<List<string>>(key);
                    if (!result.Success)// Key not in cache, so get data.
                    {
                        cacheEntry = await query();
                        await _memcachedClient.AddAsync(key, cacheEntry, 100);
                    }
                    else
                    {
                        cacheEntry = result.Value;
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            else
            {
                cacheEntry = result.Value;
            }
            return cacheEntry;
        }
        // policy
        public async Task<List<Dictionary<string, object>>> GetOrCreateOrdersListPolicy(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
            List<Dictionary<string, object>> cacheEntry;
            var result = await _memcachedClient.GetAsync<List<Dictionary<string, object>>>(key);

            if (!result.Success)// Key not in cache, so get data.
            {
                cacheEntry = await query();
                await _memcachedClient.AddAsync(key, cacheEntry, 100);
            }
            else
            {
                cacheEntry = result.Value;
            }
            return cacheEntry;
        }

        public async Task<Dictionary<string, object>> GetOrCreateOrderPolicy(Func<Task<Dictionary<string, object>>> query,
            string key)
        {
            Dictionary<string, object> cacheEntry;
            var result = await _memcachedClient.GetAsync<Dictionary<string, object>>(key);
            if (!result.Success)// Key not in cache, so get data.
            {
                cacheEntry = await query();
                await _memcachedClient.AddAsync(key, cacheEntry, 100);
            }
            else
            {
                cacheEntry = result.Value;
            }
            return cacheEntry;
        }

        public async Task<List<string>> GetOrCreateCitiesPolicy(Func<Task<List<string>>> query, string key)
        {
            List<string> cacheEntry;
            var result = await _memcachedClient.GetAsync<List<string>>(key);
            if (!result.Success)// Key not in cache, so get data.
            {
                cacheEntry = await query();
                await _memcachedClient.AddAsync(key, cacheEntry, 100);
            }
            else
            {
                cacheEntry = result.Value;
            }
            return cacheEntry;
        }
        public async Task<bool> cleanMemcacheCache()
        {
            await _memcachedClient.RemoveAsync(CacheKeys.OrderInfoWaitAndPolicyMemcache);
            await _memcachedClient.RemoveAsync(CacheKeys.OrdersInfoWaitAndPolicyMemcache);
            await _memcachedClient.RemoveAsync(CacheKeys.CityListWaitAndPolicyMemcache);

            await _memcachedClient.RemoveAsync(CacheKeys.OrderInfoPolicyMemcache);
            await _memcachedClient.RemoveAsync(CacheKeys.OrdersInfoPolicyMemcache);
            await _memcachedClient.RemoveAsync(CacheKeys.CityListPolicyMemcache);

            return true;
        }


    }
}
