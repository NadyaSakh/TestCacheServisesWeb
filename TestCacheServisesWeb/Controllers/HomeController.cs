using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enyim.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TestCacheServisesWeb.DB;
using TestCacheServisesWeb.DistributeCaching;
using TestCacheServisesWeb.Models;
using TestCacheServisesWeb.Utils;


// todo как переписать без повторений функции кеширования для разных типов?
// как использовать классы-сервисы для методов get?
namespace TestCacheServisesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private static IDistributedCache _distributedCache;

        private readonly IMemcachedClient _memcachedClient;

        public HomeController(ILogger<HomeController> logger, IDistributedCache distributedCache, 
            IMemcachedClient memcachedClient)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _memcachedClient = memcachedClient;
        }

        public static IDistributedCache GetDistributedCache()
        {
            return _distributedCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // memcached можно воспользоавться и через IDistributedCache
        [HttpGet]
        public async Task<IActionResult> OrderListMemcached()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await GetOrCreateOrdersListWaitAndPolicyMemcached(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfo);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            OrderListModel orderListModel = new OrderListModel(orderList, ellapsedTime);
            return View(orderListModel);
        }

        public async Task<IActionResult> OrderMemcached()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await GetOrCreateOrderWaitAndPolicyMemcached(queryGetOrderInfoAsync, CacheKeys.OrderInfo);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            OrderModel orderModel = new OrderModel(order, ellapsedTime);
            return View(orderModel);
        }
        public async Task<IActionResult> CitiesMemcached()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await GetOrCreateCitiesWaitAndPolicyMemcached(queryGetCitiesAsync, CacheKeys.CityList);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            CitiesModel citiesModel = new CitiesModel(cities, ellapsedTime);
            return View(citiesModel);
        }

        [HttpGet] 
        public async Task<IActionResult> OrderList()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await GetOrCreateOrdersListWaitAndPolicy(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfo);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            OrderListModel orderListModel = new OrderListModel(orderList, ellapsedTime);
            return View(orderListModel);
        }
        public async Task<IActionResult> Order()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await GetOrCreateOrderWaitAndPolicy(queryGetOrderInfoAsync, CacheKeys.OrderInfo);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            OrderModel orderModel = new OrderModel(order, ellapsedTime);
            return View(orderModel);
        }
        public async Task<IActionResult> Cities()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await GetOrCreateCitiesWaitAndPolicy(queryGetCitiesAsync, CacheKeys.CityList);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            CitiesModel citiesModel = new CitiesModel(cities, ellapsedTime);
            return View(citiesModel);
        }

        /* public async Task<List<Dictionary<string, object>>> GetOrCreateWait(Func<Task<List<Dictionary<string, object>>>> query, string key)
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
 */

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
                        await _memcachedClient.AddAsync(key, cacheEntry, 300);
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
                        await _memcachedClient.AddAsync(key, cacheEntry, 300);
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
                        await _memcachedClient.AddAsync(key, cacheEntry, 300);
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
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
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
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
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
    }
}

