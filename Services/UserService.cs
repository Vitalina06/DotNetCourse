using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UserAuthApp.Data;
using UserAuthApp.Models;

namespace UserAuthApp.Services
{
    class UserService
    {
        private UserRepository repository = new UserRepository();

        public string Register(string login, string password)
        {
            var error = ValidateLogin(login);
            if (error != null)
            {
                return error;
            }

            error = ValidatePassword(password);
            if (error != null)
            {
                return error;
            }

            var existingUser = repository.GetByLogin(login);
            if (existingUser != null)
            {
                return "Пользователь с таким логином уже существует.";
            }

            var user = new User();
            user.Login = login;
            user.PasswordHash = HashPassword(password);
            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;

            repository.Add(user);

            return null; // null значит что все прошло без ошибок
        }

        public bool Login(string login, string password)
        {
            var user = repository.GetByLogin(login);
            if (user == null)
            {
                return false;
            }

            return user.PasswordHash == HashPassword(password);
        }

        public string Edit(string login, string newLogin, string newPassword)
        {
            var user = repository.GetByLogin(login);
            if (user == null)
            {
                return "Пользователь не найден.";
            }

            if (!string.IsNullOrEmpty(newLogin))
            {
                var error = ValidateLogin(newLogin);
                if (error != null)
                {
                    return error;
                }

                user.Login = newLogin;
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                var error = ValidatePassword(newPassword);
                if (error != null)
                {
                    return error;
                }

                user.PasswordHash = HashPassword(newPassword);
            }

            user.UpdatedDate = DateTime.Now;
            repository.Update(user);

            return null;
        }

        public string Delete(string login)
        {
            var user = repository.GetByLogin(login);
            if (user == null)
            {
                return "Пользователь не найден.";
            }

            repository.Delete(user.Id);
            return null;
        }

        public List<User> GetAll()
        {
            return repository.GetAll();
        }

        public string GetUsersJson(DateTime from, DateTime to)
        {
            var users = repository.GetByDateRange(from, to);

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