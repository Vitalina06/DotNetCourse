using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace AuthService.Services
{
    class RedisCache : ICache
    {
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(Config.Settings.GetConnectionString("Redis"));

        public string Get(string key)
        {
            var db = connection.GetDatabase();
            var value = db.StringGet(key);

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return value.ToString();
        }

        public void Set(string key, string value, TimeSpan lifetime)
        {
            var db = connection.GetDatabase();
            db.StringSet(key, value, lifetime);
        }

        public void Remove(string key)
        {
            var db = connection.GetDatabase();
            db.KeyDelete(key);
        }
    }
}