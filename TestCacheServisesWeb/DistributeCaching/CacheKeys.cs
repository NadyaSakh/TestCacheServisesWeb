using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.DistributeCaching
{
    public class CacheKeys
    {
        // Memcache       
        public static string CityListPolicyMemcache { get { return "_CityListPolicyMemcache"; } }
        public static string OrderInfoPolicyMemcache { get { return "_OrderInfoPolicyMemcache"; } }
        public static string OrdersInfoPolicyMemcache { get { return "_OrdersInfoPolicyMemcache"; } }

        public static string CityListWaitAndPolicyMemcache { get { return "_CityListWaitAndPolicyMemcache"; } }
        public static string OrderInfoWaitAndPolicyMemcache { get { return "_OrderInfoWaitAndPolicyMemcache"; } }
        public static string OrdersInfoWaitAndPolicyMemcache { get { return "_OrdersInfoWaitAndPolicyMemcache"; } }
        
        // Redis
        public static string CityListRedis { get { return "_CityListRedis"; } }
        public static string OrderInfoRedis { get { return "_OrderInfoRedis"; } }
        public static string OrdersInfoRedis { get { return "_OrdersInfoRedis"; } }

        public static string CityListPolicyRedis { get { return "_CityListPolicyRedis"; } }
        public static string OrderInfoPolicyRedis { get { return "_OrderInfoPolicyRedis"; } }
        public static string OrdersInfoPolicyRedis { get { return "_OrdersInfoPolicyRedis"; } }

        public static string CityListWaitRedis { get { return "_CityListWaitRedis"; } }
        public static string OrderInfoWaitRedis { get { return "_OrderInfoWaitRedis"; } }
        public static string OrdersInfoWaitRedis { get { return "_OrdersInfoWaitRedis"; } }

        public static string CityListWaitAndPolicyRedis { get { return "_CityListWaitAndPolicyRedis"; } }
        public static string OrderInfoWaitAndPolicyRedis { get { return "_OrderInfoWaitAndPolicyRedis"; } }
        public static string OrdersInfoWaitAndPolicyRedis { get { return "_OrdersInfoWaitAndPolicyRedis"; } }
    }
}
