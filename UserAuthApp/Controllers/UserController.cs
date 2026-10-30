using System;
using UserAuthApp.Services;

namespace UserAuthApp.Controllers
{
    class UserController
    {
        private UserService userService = new UserService();

        public void CreateUser()
        {
            Console.Write("Введите логин: ");
            var login = Console.ReadLine();

            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            var error = userService.Register(login, password);

            if (error != null)
            {
                Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("Пользователь успешно создан.");
            }
        }

        public void Login()
        {
            Console.Write("Введите логин: ");
            var login = Console.ReadLine();

            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            var success = userService.Login(login, password);

            if (success)
            {
                Console.WriteLine("Авторизация успешна. Добро пожаловать, " + login + "!");
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль.");
            }
        }

        public void EditUser()
        {
            Console.Write("Введите логин пользователя, которого нужно изменить: ");
            var login = Console.ReadLine();

            Console.Write("Введите новый логин (оставьте пустым, чтобы не менять): ");
            var newLogin = Console.ReadLine();

            Console.Write("Введите новый пароль (оставьте пустым, чтобы не менять): ");
            var newPassword = Console.ReadLine();

            var error = userService.Edit(login, newLogin, newPassword);

            if (error != null)
            {
                Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("Данные пользователя обновлены.");
            }
        }

        public void DeleteUser()
        {
            Console.Write("Введите логин пользователя, которого нужно удалить: ");
            var login = Console.ReadLine();

            var error = userService.Delete(login);

            if (error != null)
            {
                Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("Пользователь удалён.");
            }
        }

        public void ShowUsers()
        {
            var users = userService.GetAll();

            if (users.Count == 0)
            {
                Console.WriteLine("Список пользователей пуст.");
                return;
            }

            foreach (var u in users)
            {
                Console.WriteLine("Id: " + u.Id + ", Login: " + u.Login +
                    ", Создан: " + u.CreatedDate + ", Обновлен: " + u.UpdatedDate);
            }
        }

        public void ShowUserCard()
        {
            Console.Write("Введите логин пользователя: ");
            var login = Console.ReadLine();

            var user = userService.GetCard(login);

            if (user == null)
            {
                Console.WriteLine("Пользователь не найден.");
                return;
            }

            Console.WriteLine("Id: " + user.Id);
            Console.WriteLine("Логин: " + user.Login);
            Console.WriteLine("Создан: " + user.CreatedDate);
            Console.WriteLine("Обновлён: " + user.UpdatedDate);
        }

        public void ShowUsersJson()
        {
            Console.Write("Введите дату начала периода (гггг-мм-дд): ");
            var fromInput = Console.ReadLine();

            Console.Write("Введите дату конца периода (гггг-мм-дд): ");
            var toInput = Console.ReadLine();

            DateTime from;
            DateTime to;

            if (!DateTime.TryParse(fromInput, out from) || !DateTime.TryParse(toInput, out to))
            {
                Console.WriteLine("Некорректный формат даты.");
                return;
            }

            var json = userService.GetUsersJson(from, to.AddDays(1).AddSeconds(-1));
            Console.WriteLine(json);
        }
    }
}