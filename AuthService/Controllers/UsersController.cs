using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.Services;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private UserService userService = new UserService();

        [HttpPost("register")]
        public IActionResult Register(UserRequest request)
        {
            var error = userService.Register(request.Login, request.Password);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok("Пользователь успешно создан.");
        }

        [HttpPost("login")]
        public IActionResult Login(UserRequest request)
        {
            var success = userService.Login(request.Login, request.Password);

            if (!success)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            return Ok("Авторизация успешна.");
        }

        [Authorize]
        [HttpPut("{login}")]
        public IActionResult Edit(string login, EditRequest request)
        {
            var error = userService.Edit(login, request.NewLogin, request.NewPassword);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok("Данные пользователя обновлены.");
        }

        [Authorize]
        [HttpDelete("{login}")]
        public IActionResult Delete(string login)
        {
            var error = userService.Delete(login);

            if (error != null)
            {
                return BadRequest(error);
            }

            return Ok("Пользователь удалён.");
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("card/{login}")]
        public IActionResult GetCard(string login)
        {
            var user = userService.GetUserCard(login);

            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            return Ok(user);
        }

        [Authorize]
        [HttpGet("json")]
        public IActionResult GetJson(DateTime from, DateTime to)
        {
            var json = userService.GetUsersJson(from, to);
            return Content(json, "application/json");
        }
    }
}