using System;
using AuthService.Models;
using AuthService.Services;
using Xunit;

namespace AuthService.Tests
{
    public class UserServiceTests
    {
        private FakeUserRepository repository = new FakeUserRepository();
        private FakeCache cache = new FakeCache();
        private UserService service;

        public UserServiceTests()
        {
            service = new UserService(repository, cache);
        }

        //  Регистрация 

        [Fact]
        public void Register_ValidData_CreatesUser()
        {
            var result = service.Register("vitalina", "123456");

            Assert.Null(result);
            Assert.Single(repository.Users);
            Assert.Equal("vitalina", repository.Users[0].Login);
        }

        [Fact]
        public void Register_PasswordIsHashed()
        {
            service.Register("vitalina", "123456");

            var hash = repository.Users[0].PasswordHash;

            Assert.NotEqual("123456", hash);
            Assert.Equal("8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", hash);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Register_EmptyLogin_ReturnsError(string login)
        {
            var result = service.Register(login, "123456");

            Assert.Equal("Логин не может быть пустым.", result);
            Assert.Empty(repository.Users);
        }

        [Fact]
        public void Register_ShortLogin_ReturnsError()
        {
            var result = service.Register("ab", "123456");

            Assert.Equal("Логин должен содержать минимум 3 символа.", result);
            Assert.Empty(repository.Users);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Register_EmptyPassword_ReturnsError(string password)
        {
            var result = service.Register("vitalina", password);

            Assert.Equal("Пароль не может быть пустым.", result);
            Assert.Empty(repository.Users);
        }

        [Fact]
        public void Register_ShortPassword_ReturnsError()
        {
            var result = service.Register("vitalina", "12345");

            Assert.Equal("Пароль должен содержать минимум 6 символов.", result);
            Assert.Empty(repository.Users);
        }

        [Fact]
        public void Register_DuplicateLogin_ReturnsError()
        {
            service.Register("vitalina", "123456");

            var result = service.Register("vitalina", "654321");

            Assert.Equal("Пользователь с таким логином уже существует.", result);
            Assert.Single(repository.Users);
        }

        // Авторизация

        [Fact]
        public void Login_CorrectPassword_ReturnsTrue()
        {
            service.Register("vitalina", "123456");

            var result = service.Login("vitalina", "123456");

            Assert.True(result);
        }

        [Fact]
        public void Login_WrongPassword_ReturnsFalse()
        {
            service.Register("vitalina", "123456");

            var result = service.Login("vitalina", "wrong_password");

            Assert.False(result);
        }

        [Fact]
        public void Login_UnknownUser_ReturnsFalse()
        {
            var result = service.Login("ghost", "123456");

            Assert.False(result);
        }

        // Редактирование

        [Fact]
        public void Edit_NewLogin_ChangesLogin()
        {
            service.Register("vitalina", "123456");

            var result = service.Edit("vitalina", "vita_new", "");

            Assert.Null(result);
            Assert.Equal("vita_new", repository.Users[0].Login);
        }

        [Fact]
        public void Edit_NewPassword_ChangesPassword()
        {
            service.Register("vitalina", "123456");

            var result = service.Edit("vitalina", "", "654321");

            Assert.Null(result);
            Assert.True(service.Login("vitalina", "654321"));
            Assert.False(service.Login("vitalina", "123456"));
        }

        [Fact]
        public void Edit_ShortNewPassword_ReturnsErrorAndKeepsOldPassword()
        {
            service.Register("vitalina", "123456");

            var result = service.Edit("vitalina", "", "123");

            Assert.Equal("Пароль должен содержать минимум 6 символов.", result);
            Assert.True(service.Login("vitalina", "123456"));
        }

        [Fact]
        public void Edit_UnknownUser_ReturnsError()
        {
            var result = service.Edit("ghost", "new_login", "");

            Assert.Equal("Пользователь не найден.", result);
        }

        // Удаление

        [Fact]
        public void Delete_ExistingUser_RemovesUser()
        {
            service.Register("vitalina", "123456");

            var result = service.Delete("vitalina");

            Assert.Null(result);
            Assert.Empty(repository.Users);
        }

        [Fact]
        public void Delete_UnknownUser_ReturnsError()
        {
            var result = service.Delete("ghost");

            Assert.Equal("Пользователь не найден.", result);
        }

        // Карточка пользователя и кэш

        [Fact]
        public void GetUserCard_SecondCall_TakesUserFromCache()
        {
            service.Register("vitalina", "123456");
            repository.GetByLoginCalls = 0;

            var first = service.GetUserCard("vitalina");
            var second = service.GetUserCard("vitalina");

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.Equal("vitalina", second.Login);
            Assert.Equal(1, repository.GetByLoginCalls);
            Assert.NotNull(cache.Get("user:vitalina"));
        }

        [Fact]
        public void GetUserCard_UnknownUser_ReturnsNullAndDoesNotCache()
        {
            var result = service.GetUserCard("ghost");

            Assert.Null(result);
            Assert.Null(cache.Get("user:ghost"));
        }

        [Fact]
        public void Edit_RemovesUserCardFromCache()
        {
            service.Register("vitalina", "123456");
            service.GetUserCard("vitalina");
            Assert.NotNull(cache.Get("user:vitalina"));

            service.Edit("vitalina", "", "654321");

            Assert.Null(cache.Get("user:vitalina"));
        }

        [Fact]
        public void Delete_RemovesUserCardFromCache()
        {
            service.Register("vitalina", "123456");
            service.GetUserCard("vitalina");
            Assert.NotNull(cache.Get("user:vitalina"));

            service.Delete("vitalina");

            Assert.Null(cache.Get("user:vitalina"));
        }

        // JSON за период

        [Fact]
        public void GetUsersJson_ReturnsOnlyUsersFromPeriod()
        {
            var oldUser = new User();
            oldUser.Login = "old_user";
            oldUser.PasswordHash = "hash1";
            oldUser.CreatedDate = new DateTime(2026, 1, 10);
            oldUser.UpdatedDate = new DateTime(2026, 1, 10);
            repository.Add(oldUser);

            var newUser = new User();
            newUser.Login = "new_user";
            newUser.PasswordHash = "hash2";
            newUser.CreatedDate = new DateTime(2026, 3, 10);
            newUser.UpdatedDate = new DateTime(2026, 3, 10);
            repository.Add(newUser);

            var json = service.GetUsersJson(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

            Assert.Contains("old_user", json);
            Assert.DoesNotContain("new_user", json);
        }

        [Fact]
        public void GetUsersJson_DoesNotContainPasswordHash()
        {
            service.Register("vitalina", "123456");

            var json = service.GetUsersJson(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));

            Assert.Contains("vitalina", json);
            Assert.DoesNotContain("PasswordHash", json);
        }
    }
}