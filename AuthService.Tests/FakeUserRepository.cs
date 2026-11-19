using System;
using System.Collections.Generic;
using System.Linq;
using AuthService.Data;
using AuthService.Models;

namespace AuthService.Tests
{
    class FakeUserRepository : IUserRepository
    {
        public List<User> Users = new List<User>();
        public int GetByLoginCalls = 0;
        private int nextId = 1;

        public void Add(User user)
        {
            user.Id = nextId;
            nextId++;
            Users.Add(user);
        }

        public User GetByLogin(string login)
        {
            GetByLoginCalls++;
            return Users.FirstOrDefault(u => u.Login == login);
        }

        public List<User> GetAll()
        {
            return Users;
        }

        public List<User> GetByDateRange(DateTime from, DateTime to)
        {
            return Users
                .Where(u => (u.CreatedDate >= from && u.CreatedDate <= to) ||
                            (u.UpdatedDate >= from && u.UpdatedDate <= to))
                .ToList();
        }

        public void Update(User user)
        {
            // в списке лежит тот же самый объект, поэтому отдельно обновлять нечего
        }

        public void Delete(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            Users.Remove(user);
        }
    }
}