using System;
using Microsoft.Extensions.Configuration;

namespace UserAuthApp
{
    static class Config
    {
        public static IConfiguration Settings = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
    }
}