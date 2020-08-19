using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.Models
{
    public class OrderListModel
    {
        public List<Dictionary<string, object>> _orderList { get; set; }

        public string _ellapsedTime { get; set; }

        public OrderListModel(List<Dictionary<string, object>> orderList, string ellapsedTime)
        {
            _orderList = orderList;
            _ellapsedTime = ellapsedTime;
        }
    }
}
