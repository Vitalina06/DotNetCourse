using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using Serilog;
using UserAuthApp.Models;

namespace UserAuthApp.Services
{
    class UserService
    {
        private HttpClient client = new HttpClient();
        private string unavailableMessage = "Сервис авторизации недоступен. Попробуйте позже.";

        public UserService()
        {
            client.BaseAddress = new Uri("http://localhost:5100/");
        }

        public string Register(string login, string password)
        {
            try
            {
                var body = new { Login = login, Password = password };
                var response = client.PostAsJsonAsync("api/users/register", body).Result;

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при регистрации");
                return unavailableMessage;
            }
        }

        public bool Login(string login, string password)
        {
            try
            {
                var body = new { Login = login, Password = password };
                var response = client.PostAsJsonAsync("api/users/login", body).Result;

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при авторизации");
                return false;
            }
        }

        public string Edit(string login, string newLogin, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return "Логин не может быть пустым.";
            }

            try
            {
                var body = new { NewLogin = newLogin, NewPassword = newPassword };
                var response = client.PutAsJsonAsync("api/users/" + Uri.EscapeDataString(login), body).Result;

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при редактировании");
                return unavailableMessage;
            }
        }

        public string Delete(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return "Логин не может быть пустым.";
            }

            try
            {
                var response = client.DeleteAsync("api/users/" + Uri.EscapeDataString(login)).Result;

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при удалении");
                return unavailableMessage;
            }
        }

        public List<User> GetAll()
        {
            try
            {
                var response = client.GetAsync("api/users").Result;

                if (!response.IsSuccessStatusCode)
                {
                    return new List<User>();
                }

                var users = response.Content.ReadFromJsonAsync<List<User>>().Result;
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при получении списка");
                return new List<User>();
            }
        }

        public User GetCard(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            try
            {
                var response = client.GetAsync("api/users/card/" + Uri.EscapeDataString(login)).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return response.Content.ReadFromJsonAsync<User>().Result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при получении карточки");
                return null;
            }
        }

        public string GetUsersJson(DateTime from, DateTime to)
        {
            try
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
            catch (Exception ex)
            {
                Log.Error(ex, "Не удалось обратиться к AuthService при получении JSON");
                return "[]";
            }
        }
    }
}