using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Serilog;
using AuthService.Data;
using AuthService.Models;

namespace AuthService.Services
{
    class UserService
    {
        private UserRepository repository = new UserRepository();
        private RedisCache cache = new RedisCache();

        public string Register(string login, string password)
        {
            var error = ValidateLogin(login);
            if (error != null)
            {
                Log.Warning("Регистрация не удалась: {Error}. Логин: {Login}", error, login);
                return error;
            }

            error = ValidatePassword(password);
            if (error != null)
            {
                Log.Warning("Регистрация не удалась: {Error}. Логин: {Login}", error, login);
                return error;
            }

            User existingUser;
            try
            {
                existingUser = repository.GetByLogin(login);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обращении к базе данных при регистрации пользователя {Login}", login);
                return "Произошла ошибка. Попробуйте позже.";
            }

            if (existingUser != null)
            {
                Log.Warning("Попытка регистрации с уже существующим логином: {Login}", login);
                return "Пользователь с таким логином уже существует.";
            }

            var user = new User();
            user.Login = login;
            user.PasswordHash = HashPassword(password);
            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;

            try
            {
                repository.Add(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сохранении нового пользователя {Login}", login);
                return "Произошла ошибка. Попробуйте позже.";
            }

            Log.Information("Зарегистрирован новый пользователь: {Login}", login);

            return null;
        }

        public bool Login(string login, string password)
        {
            User user;
            try
            {
                user = repository.GetByLogin(login);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обращении к базе данных при авторизации {Login}", login);
                return false;
            }

            if (user == null)
            {
                Log.Warning("Попытка авторизации с несуществующим логином: {Login}", login);
                return false;
            }

            if (user.PasswordHash != HashPassword(password))
            {
                Log.Warning("Неверный пароль при авторизации пользователя: {Login}", login);
                return false;
            }

            Log.Information("Пользователь авторизован: {Login}", login);
            return true;
        }

        public string Edit(string login, string newLogin, string newPassword)
        {
            User user;
            try
            {
                user = repository.GetByLogin(login);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обращении к базе данных при редактировании {Login}", login);
                return "Произошла ошибка. Попробуйте позже.";
            }

            if (user == null)
            {
                Log.Warning("Попытка редактирования несуществующего пользователя: {Login}", login);
                return "Пользователь не найден.";
            }

            if (!string.IsNullOrEmpty(newLogin))
            {
                var error = ValidateLogin(newLogin);
                if (error != null)
                {
                    Log.Warning("Редактирование не удалось: {Error}. Логин: {Login}", error, login);
                    return error;
                }

                user.Login = newLogin;
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                var error = ValidatePassword(newPassword);
                if (error != null)
                {
                    Log.Warning("Редактирование не удалось: {Error}. Логин: {Login}", error, login);
                    return error;
                }

                user.PasswordHash = HashPassword(newPassword);
            }

            user.UpdatedDate = DateTime.Now;

            try
            {
                repository.Update(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сохранении изменений пользователя {Login}", login);
                return "Произошла ошибка. Попробуйте позже.";
            }

            cache.Remove("user:" + login);
            Log.Information("Данные пользователя обновлены: {Login}, кэш карточки сброшен", login);

            return null;
        }

        public string Delete(string login)
        {
            User user;
            try
            {
                user = repository.GetByLogin(login);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обращении к базе данных при удалении {Login}", login);
                return "Произошла ошибка. Попробуйте позже.";
            }

            if (user == null)
            {
                Log.Warning("Попытка удаления несуществующего пользователя: {Login}", login);
                return "Пользователь не найден.";
            }

            try
            {
                repository.Delete(user.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при удалении пользователя {Login}", login);
                return "Произошла ошибка. Попробуйте позже.";
            }

            cache.Remove("user:" + login);
            Log.Information("Пользователь удалён: {Login}, кэш карточки сброшен", login);

            return null;
        }

        public User GetUserCard(string login)
        {
            var key = "user:" + login;

            var cached = cache.Get(key);
            if (cached != null)
            {
                Log.Information("Карточка пользователя {Login} получена из Redis", login);
                return JsonSerializer.Deserialize<User>(cached);
            }

            User user;
            try
            {
                user = repository.GetByLogin(login);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении карточки пользователя {Login}", login);
                return null;
            }

            if (user == null)
            {
                Log.Warning("Запрошена карточка несуществующего пользователя: {Login}", login);
                return null;
            }

            cache.Set(key, JsonSerializer.Serialize(user), TimeSpan.FromMinutes(5));
            Log.Information("Карточка пользователя {Login} получена из PostgreSQL и сохранена в Redis", login);

            return user;
        }

        public List<User> GetAll()
        {
            try
            {
                return repository.GetAll();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении списка пользователей");
                return new List<User>();
            }
        }

        public string GetUsersJson(DateTime from, DateTime to)
        {
            List<User> users;
            try
            {
                users = repository.GetByDateRange(from, to);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при получении пользователей за период {From} - {To}", from, to);
                return "[]";
            }

            Log.Information("Запрошен список пользователей за период {From} - {To}, найдено {Count}", from, to, users.Count);

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;

            return JsonSerializer.Serialize(users, options);
        }

        private string ValidateLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return "Логин не может быть пустым.";
            }

            if (login.Length < 3)
            {
                return "Логин должен содержать минимум 3 символа.";
            }

            return null;
        }

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Пароль не может быть пустым.";
            }

            if (password.Length < 6)
            {
                return "Пароль должен содержать минимум 6 символов.";
            }

            return null;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();

                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}