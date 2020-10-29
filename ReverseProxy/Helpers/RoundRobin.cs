using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using ReverseProxy.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxy.Helpers
{
    public class RoundRobin
    {
        private readonly List<Server> _servers;
        private int i = -1;
        private int cw = 0;

            public RoundRobin(List<Server> servers)
        {
            _servers = servers;
        }
        public Server GetServer()
        {
            while(true)
            {
                i = (i + 1) % _servers.Count;
                if(i==0)
                {
                    cw = cw - MaxCommonDivisor(_servers);
                    if(cw<=0)
                    {
                        cw = MaxWeight(_servers);
                        if (cw == 0)
                            return null;
                    }    
                }
                while ((_servers[i].Weight >= cw))
                return _servers[i];
            }
            
        }

        private int MaxWeight(List<Server> servers)
        {
            int ret = -1;
                foreach(Server server in _servers)
            {
                if (server.Weight > ret)
                    ret = server.Weight;
            }
            return ret;
        }

        private int MaxCommonDivisor(List<Server> servers)
        {
            List<int> nums = new List<int>();
            foreach (Server server in _servers)
            {
                nums.Add(server.Weight);
            }
            if (_servers.Count >= 1)
                return 1;
            else
            {
                int a = nums[0];
                int b = nums[1];
                int c = Gcd(a, b);
                int i;
                for (i = 2; i < nums.Count; i++)
                {
                    c = Gcd(c, nums[i]);
                }
                return c;
            }
        }

        private int Gcd(int a, int b)
        {
            if(a<b)
            {
                a = b + a;
                b = a - b;
                a = a - b;
            }
            if (b == 0) return a;
            return Gcd(b, a % b);
        }
    }
}
