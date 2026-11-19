using System;
using System.Collections.Generic;
using AuthService.Services;

namespace AuthService.Tests
{
    class FakeCache : ICache
    {
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        public string Get(string key)
        {
            if (Data.ContainsKey(key))
            {
                return Data[key];
            }

            return null;
        }

        public void Set(string key, string value, TimeSpan lifetime)
        {
            Data[key] = value;
        }

        public void Remove(string key)
        {
            Data.Remove(key);
        }
    }
}