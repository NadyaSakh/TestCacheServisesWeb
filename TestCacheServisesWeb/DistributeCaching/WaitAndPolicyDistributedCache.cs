using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestCacheServisesWeb.Controllers;

namespace TestCacheServisesWeb.DistributeCaching
{
    class WaitAndPolicyDistributedCache<TIem>
    {
        private ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        private IDistributedCache _distributedCache = HomeController.GetDistributedCache();

        public async Task<List<Dictionary<string, object>>> GetOrCreateAsync(Func<Task<List<Dictionary<string, object>>>> query, string key)
        {
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
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
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
    }
}
