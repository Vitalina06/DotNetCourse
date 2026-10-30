using System;
using Serilog;
using StackExchange.Redis;

namespace AuthService.Services
{
    class RedisCache
    {
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");

        public string Get(string key)
        {
            try
            {
                var db = connection.GetDatabase();
                var value = db.StringGet(key);

                if (value.IsNullOrEmpty)
                {
                    return null;
                }

                return value.ToString();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Redis недоступен при чтении ключа {Key}", key);
                return null;
            }
        }

        public void Set(string key, string value, TimeSpan lifetime)
        {
            try
            {
                var db = connection.GetDatabase();
                db.StringSet(key, value, lifetime);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Redis недоступен при записи ключа {Key}", key);
            }
        }

        public void Remove(string key)
        {
            try
            {
                var db = connection.GetDatabase();
                db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Redis недоступен при удалении ключа {Key}", key);
            }
        }
    }
}