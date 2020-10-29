using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxy.Services
{
    public class Server
    {
        public string ID { get; set; }
        public string ServerName { get; set; }
        public string Location { get; set; }
        public int Weight { get; set; }
        public string Service { get; set; }
    }
}
