using System;

namespace AuthService.Services
{
    interface ICache
    {
        string Get(string key);
        void Set(string key, string value, TimeSpan lifetime);
        void Remove(string key);
    }
}