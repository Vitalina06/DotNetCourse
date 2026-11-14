using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using UserAuthApp.Models;

namespace UserAuthApp.Services
{
    class UserService
    {
        private HttpClient client = new HttpClient();

        public UserService()
        {
            client.BaseAddress = new Uri(Config.Settings["AuthServiceUrl"]);
            client.Timeout = TimeSpan.FromMinutes(5);
        }

        public string Register(string login, string password)
        {
            var body = new { Login = login, Password = password };
            var response = client.PostAsJsonAsync("api/users/register", body).Result;

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public bool Login(string login, string password)
        {
            var body = new { Login = login, Password = password };
            var response = client.PostAsJsonAsync("api/users/login", body).Result;

            return response.IsSuccessStatusCode;
        }

        public string Edit(string login, string newLogin, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return "Логин не может быть пустым.";
            }

            var body = new { NewLogin = newLogin, NewPassword = newPassword };
            var response = client.PutAsJsonAsync("api/users/" + Uri.EscapeDataString(login), body).Result;

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public string Delete(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return "Логин не может быть пустым.";
            }

            var response = client.DeleteAsync("api/users/" + Uri.EscapeDataString(login)).Result;

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public List<User> GetAll()
        {
            var response = client.GetAsync("api/users").Result;

            if (!response.IsSuccessStatusCode)
            {
                return new List<User>();
            }

            return response.Content.ReadFromJsonAsync<List<User>>().Result;
        }

        public User GetCard(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            var response = client.GetAsync("api/users/card/" + Uri.EscapeDataString(login)).Result;

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return response.Content.ReadFromJsonAsync<User>().Result;
        }

        public string GetUsersJson(DateTime from, DateTime to)
        {
            var fromText = from.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var toText = to.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            var url = "api/users/json?from=" + Uri.EscapeDataString(fromText) +
                "&to=" + Uri.EscapeDataString(toText);

            var response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                return "[]";
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public string AskAi(string question)
        {
            var body = new { Question = question };
            var response = client.PostAsJsonAsync("api/ai/ask", body).Result;

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}