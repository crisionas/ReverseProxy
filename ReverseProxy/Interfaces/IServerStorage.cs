using ReverseProxy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxy.Interfaces
{
    public interface IServerStorage
    {
        public Server GetServer(string service);
    }
}
