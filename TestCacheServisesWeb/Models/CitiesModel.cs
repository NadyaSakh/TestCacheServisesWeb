using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.Models
{
    public class CitiesModel
    {
        public List<string> _cities { get; set; }

        public string _ellapsedTime { get; set; }

        public CitiesModel(List<string> cities, string ellapsedTime)
        {
            _cities = cities;
            _ellapsedTime = ellapsedTime;
        }
    }
}
