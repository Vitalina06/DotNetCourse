using System;
using UserAuthApp.Controllers;

namespace UserAuthApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new UserController();
            var exit = false;

            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("=== Меню ===");
                Console.WriteLine("1. Создать пользователя");
                Console.WriteLine("2. Авторизация");
                Console.WriteLine("3. Редактировать пользователя");
                Console.WriteLine("4. Удалить пользователя");
                Console.WriteLine("5. Показать всех пользователей");
                Console.WriteLine("6. Показать пользователей в JSON за период");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        controller.CreateUser();
                        break;
                    case "2":
                        controller.Login();
                        break;
                    case "3":
                        controller.EditUser();
                        break;
                    case "4":
                        controller.DeleteUser();
                        break;
                    case "5":
                        controller.ShowUsers();
                        break;
                    case "6":
                        controller.ShowUsersJson();
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
    }
}
