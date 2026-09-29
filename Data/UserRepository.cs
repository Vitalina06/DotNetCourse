using System;
using System.Collections.Generic;
using System.Linq;
using UserAuthApp.Models;

namespace UserAuthApp.Data
{
    class UserRepository
    {
        public void Add(User user)
        {
            using (var context = new AppDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public User GetByLogin(string login)
        {
            using (var context = new AppDbContext())
            {
                return context.Users.FirstOrDefault(u => u.Login == login);
            }
        }

        public List<User> GetAll()
        {
            using (var context = new AppDbContext())
            {
                return context.Users.OrderBy(u => u.Id).ToList();
            }
        }

        public List<User> GetByDateRange(DateTime from, DateTime to)
        {
            using (var context = new AppDbContext())
            {
                return context.Users
                    .Where(u => (u.CreatedDate >= from && u.CreatedDate <= to) ||
                                (u.UpdatedDate >= from && u.UpdatedDate <= to))
                    .OrderBy(u => u.Id)
                    .ToList();
            }
        }

        public void Update(User user)
        {
            using (var context = new AppDbContext())
            {
                var existingUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    return;
                }

                existingUser.Login = user.Login;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.UpdatedDate = user.UpdatedDate;

                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                }
            }
        }
    }
}