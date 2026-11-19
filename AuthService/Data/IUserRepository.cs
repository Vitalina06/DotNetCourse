using System;
using System.Collections.Generic;
using AuthService.Models;

namespace AuthService.Data
{
    interface IUserRepository
    {
        void Add(User user);
        User GetByLogin(string login);
        List<User> GetAll();
        List<User> GetByDateRange(DateTime from, DateTime to);
        void Update(User user);
        void Delete(int id);
    }
}