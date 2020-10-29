using Microsoft.Extensions.Configuration;
using ReverseProxy.Helpers;
using ReverseProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReverseProxy.Services
{
    public class ServerStorage : IServerStorage
    {

        private readonly IConfiguration _config;
        private Dictionary<string, List<Server>> storage;
        private Dictionary<string, RoundRobin> loadbalancing;

        public ServerStorage(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _config = configuration;
            storage = new Dictionary<string, List<Server>>();
            GetServerList();
                
        }

        private void GetServerList()
        {
            List<Server> serversList = new List<Server>();
            _config.GetSection("servers").Bind(serversList);

            storage = serversList.GroupBy(m => m.Service)
                .ToDictionary(g => g.Key, g => g.ToList());
            ServersLoadBalancing();
        }

        public Server GetServer(string service)
        {
            var branch = loadbalancing
                .Where(m => m.Key == service)
                .Select(m => m.Value)
                .FirstOrDefault();

            if (branch == null)
            {
                branch = loadbalancing.Where(m => m.Key =="").Select(m => m.Value).FirstOrDefault();
            }

            var selected = branch.GetServer();
            return selected;
        }

        private void ServersLoadBalancing()
        {
            loadbalancing = new Dictionary<string, RoundRobin>();

            foreach(var item in storage)
            {
                loadbalancing.Add(item.Key, new RoundRobin(item.Value));
            }
        }

    }
}
