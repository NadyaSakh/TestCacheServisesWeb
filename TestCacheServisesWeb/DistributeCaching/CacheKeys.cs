using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.DistributeCaching
{
    public class CacheKeys
    {
        public static string City { get { return "_City"; } }
        public static string CityList { get { return "_CityList"; } }
        public static string OrderInfo { get { return "_OrderInfo"; } }
        public static string OrdersInfo { get { return "_OrdersInfo"; } }
    }
}
