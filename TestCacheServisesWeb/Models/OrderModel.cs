using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.Models
{
    public class OrderModel
    {
        public Dictionary<string, object> _order { get; set; }

        public string _ellapsedTime { get; set; }

        public OrderModel(Dictionary<string, object> order, string ellapsedTime)
        {
            _order = order;
            _ellapsedTime = ellapsedTime;
        }
    }
}
