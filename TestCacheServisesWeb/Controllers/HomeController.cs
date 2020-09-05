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
using TestCacheServisesWeb.Services;
using TestCacheServisesWeb.Utils;


namespace TestCacheServisesWeb.Controllers
{
    public class HomeController : Controller
    {
        MemcacheCacheService _memcacheCacheService;
        RedisCacheService _redisCacheService;
       
        public HomeController(MemcacheCacheService memcacheCacheService, RedisCacheService redisCacheService)
        {
            _memcacheCacheService = memcacheCacheService;
            _redisCacheService = redisCacheService;
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

        [HttpGet]
        public async Task<IActionResult> OrderListMemcached()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await _memcacheCacheService.GetOrCreateOrdersListWaitAndPolicyMemcached(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfoWaitAndPolicyMemcache);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            OrderListModel orderListModel = new OrderListModel(orderList, ellapsedTime);
            return View(orderListModel);
        }

        [HttpGet]
        public async Task<IActionResult> OrderMemcached()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await _memcacheCacheService.GetOrCreateOrderWaitAndPolicyMemcached(
                queryGetOrderInfoAsync, CacheKeys.OrderInfoWaitAndPolicyMemcache);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            OrderModel orderModel = new OrderModel(order, ellapsedTime);
            return View(orderModel);
        }

        [HttpGet]
        public async Task<IActionResult> CitiesMemcached()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await _memcacheCacheService.GetOrCreateCitiesWaitAndPolicyMemcached(
                queryGetCitiesAsync, CacheKeys.CityListWaitAndPolicyMemcache);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            CitiesModel citiesModel = new CitiesModel(cities, ellapsedTime);
            return View(citiesModel);
        }

        // policy
        [HttpGet]
        public async Task<string> OrderListPolicyMemcached()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await _memcacheCacheService.GetOrCreateOrdersListPolicy(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfoPolicyMemcache);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }

        [HttpGet]
        public async Task<string> OrderPolicyMemcached()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await _memcacheCacheService.GetOrCreateOrderPolicy(
                queryGetOrderInfoAsync, CacheKeys.OrderInfoPolicyMemcache);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }

        [HttpGet]
        public async Task<string> CitiesPolicyMemcached()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await _memcacheCacheService.GetOrCreateCitiesPolicy(
                queryGetCitiesAsync, CacheKeys.CityListPolicyMemcache);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }

        [HttpGet]
        public async Task<String> CleanMemcachedCache()
        {
            bool res = await _memcacheCacheService.cleanMemcacheCache();
            if (res)
            {
                return "Кеш memcache отчищен";
            }
            else
            {
                return "Кеш Не отчищен";
            }
        }

        [HttpGet]
        public String CleanRedisCache()
        {
             _redisCacheService.cleanRedisCache();
             return "Кеш redis отчищен";
        }

        [HttpGet] 
        public async Task<IActionResult> OrderList()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await _redisCacheService.GetOrCreateOrdersListWaitAndPolicy(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfoWaitAndPolicyRedis);
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
            Dictionary<string, object> order = await _redisCacheService.GetOrCreateOrderWaitAndPolicy(
                queryGetOrderInfoAsync, CacheKeys.OrderInfoWaitAndPolicyRedis);

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
            List<string> cities = await _redisCacheService.GetOrCreateCitiesWaitAndPolicy(queryGetCitiesAsync, 
                CacheKeys.CityListWaitAndPolicyRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            CitiesModel citiesModel = new CitiesModel(cities, ellapsedTime);
            return View(citiesModel);
        }

        public async Task<string> OrdersSimpleRedis()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await _redisCacheService.GetOrCreateOrdersSimple(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfoRedis);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
        public async Task<string> OrderSimpleRedis()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await _redisCacheService.GetOrCreateOrderSimple(
                queryGetOrderInfoAsync, CacheKeys.OrderInfoRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
        public async Task<string> CitiesSimpleRedis()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await _redisCacheService.GetOrCreateCitiesSimple(queryGetCitiesAsync,
                CacheKeys.CityListRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }

        // policy
        public async Task<string> OrdersPolicyRedis()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await _redisCacheService.GetOrCreateOrdersPolicy(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfoPolicyRedis);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
        public async Task<string> OrderPolicyRedis()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await _redisCacheService.GetOrCreateOrderPolicy(
                queryGetOrderInfoAsync, CacheKeys.OrderInfoPolicyRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
        public async Task<string> CitiesPolicyRedis()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await _redisCacheService.GetOrCreateCitiesPolicy(queryGetCitiesAsync,
                CacheKeys.CityListPolicyRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }

        // wait
        public async Task<string> OrdersWaitRedis()
        {
            Func<Task<List<Dictionary<string, object>>>> queryGetOrdersInfoAsync = () => GetData.QueryGetOrdersInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();

            List<Dictionary<string, object>> orderList = await _redisCacheService.GetOrCreateOrdersWait(
                queryGetOrdersInfoAsync, CacheKeys.OrdersInfoWaitRedis);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
        public async Task<string> OrderWaitRedis()
        {
            Func<Task<Dictionary<string, object>>> queryGetOrderInfoAsync = () => GetData.QueryGetOrderInfoAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Dictionary<string, object> order = await _redisCacheService.GetOrCreateOrderWait(
                queryGetOrderInfoAsync, CacheKeys.OrderInfoWaitRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
        public async Task<string> CitiesWaitRedis()
        {
            Func<Task<List<string>>> queryGetCitiesAsync = () => GetData.QueryGetCitiesAsync();
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<string> cities = await _redisCacheService.GetOrCreateCitiesWait(queryGetCitiesAsync,
                CacheKeys.CityListWaitRedis);

            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            string ellapsedTime = TimeUtils.showEllapsedTime(ts1);

            return ellapsedTime;
        }
    }
}

