using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UserAuthApp
{
    class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
    }

    class Program
    {
        static List<User> users = new List<User>();
        static int nextId = 1;

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("=== Меню ===");
                Console.WriteLine("1. Создать пользователя");
                Console.WriteLine("2. Авторизация");
                Console.WriteLine("3. Редактировать пользователя");
                Console.WriteLine("4. Удалить пользователя");
                Console.WriteLine("5. Показать всех пользователей");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateUser();
                        break;
                    case "2":
                        Login();
                        break;
                    case "3":
                        EditUser();
                        break;
                    case "4":
                        DeleteUser();
                        break;
                    case "5":
                        ShowUsers();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный пункт меню.");
                        break;
                }
            }
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        static void CreateUser()
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();

            foreach (User u in users)
            {
                if (u.Login == login)
                {
                    Console.WriteLine("Пользователь с таким логином уже существует.");
                    return;
                }
            }

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            User newUser = new User();
            newUser.Id = nextId;
            newUser.Login = login;
            newUser.PasswordHash = HashPassword(password);

            users.Add(newUser);
            nextId++;

            Console.WriteLine("Пользователь успешно создан.");
        }

        static void Login()
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            string hash = HashPassword(password);

            foreach (User u in users)
            {
                if (u.Login == login && u.PasswordHash == hash)
                {
                    Console.WriteLine("Авторизация успешна. Добро пожаловать, " + u.Login + "!");
                    return;
                }
            }

            Console.WriteLine("Неверный логин или пароль.");
        }

        static void EditUser()
        {
            Console.Write("Введите логин пользователя, которого нужно изменить: ");
            string login = Console.ReadLine();

            User foundUser = null;
            foreach (User u in users)
            {
                if (u.Login == login)
                {
                    foundUser = u;
                    break;
                }
            }

            if (foundUser == null)
            {
                Console.WriteLine("Пользователь не найден.");
                return;
            }

            Console.Write("Введите новый логин (оставьте пустым, чтобы не менять): ");
            string newLogin = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLogin))
            {
                foundUser.Login = newLogin;
            }

            Console.Write("Введите новый пароль (оставьте пустым, чтобы не менять): ");
            string newPassword = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPassword))
            {
                foundUser.PasswordHash = HashPassword(newPassword);
            }

            Console.WriteLine("Данные пользователя обновлены.");
        }

        static void DeleteUser()
        {
            Console.Write("Введите логин пользователя, которого нужно удалить: ");
            string login = Console.ReadLine();

            User foundUser = null;
            foreach (User u in users)
            {
                if (u.Login == login)
                {
                    foundUser = u;
                    break;
                }
            }

            if (foundUser == null)
            {
                Console.WriteLine("Пользователь не найден.");
                return;
            }

            users.Remove(foundUser);
            Console.WriteLine("Пользователь удалён.");
        }

        static void ShowUsers()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("Список пользователей пуст.");
                return;
            }

            foreach (User u in users)
            {
                Console.WriteLine("Id: " + u.Id + ", Login: " + u.Login + ", PasswordHash: " + u.PasswordHash);
            }
        }
    }
}
