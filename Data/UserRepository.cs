using System;
using System.Collections.Generic;
using Npgsql;
using UserAuthApp.Models;

namespace UserAuthApp.Data
{
    class UserRepository
    {
        private string connectionString = "Host=localhost;Port=5432;Database=userauthdb;Username=postgres;Password=postgres";

        public void Add(User user)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    "INSERT INTO users (login, password_hash, created_date, updated_date) " +
                    "VALUES (@login, @passwordHash, @createdDate, @updatedDate) RETURNING id",
                    connection);

                command.Parameters.AddWithValue("login", user.Login);
                command.Parameters.AddWithValue("passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("createdDate", user.CreatedDate);
                command.Parameters.AddWithValue("updatedDate", user.UpdatedDate);

                var newId = command.ExecuteScalar();
                user.Id = Convert.ToInt32(newId);
            }
        }

        public User GetByLogin(string login)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    "SELECT id, login, password_hash, created_date, updated_date FROM users WHERE login = @login",
                    connection);
                command.Parameters.AddWithValue("login", login);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadUser(reader);
                    }
                }
            }

            return null;
        }

        public List<User> GetAll()
        {
            var result = new List<User>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    "SELECT id, login, password_hash, created_date, updated_date FROM users ORDER BY id",
                    connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadUser(reader));
                    }
                }
            }

            return result;
        }

        public List<User> GetByDateRange(DateTime from, DateTime to)
        {
            var result = new List<User>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    "SELECT id, login, password_hash, created_date, updated_date FROM users " +
                    "WHERE (created_date BETWEEN @from AND @to) OR (updated_date BETWEEN @from AND @to) " +
                    "ORDER BY id",
                    connection);

                command.Parameters.AddWithValue("from", from);
                command.Parameters.AddWithValue("to", to);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadUser(reader));
                    }
                }
            }

            return result;
        }

        public void Update(User user)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand(
                    "UPDATE users SET login = @login, password_hash = @passwordHash, updated_date = @updatedDate WHERE id = @id",
                    connection);

                command.Parameters.AddWithValue("login", user.Login);
                command.Parameters.AddWithValue("passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("updatedDate", user.UpdatedDate);
                command.Parameters.AddWithValue("id", user.Id);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM users WHERE id = @id", connection);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
            }
        }

        private User ReadUser(NpgsqlDataReader reader)
        {
            var user = new User();
            user.Id = reader.GetInt32(0);
            user.Login = reader.GetString(1);
            user.PasswordHash = reader.GetString(2);
            user.CreatedDate = reader.GetDateTime(3);
            user.UpdatedDate = reader.GetDateTime(4);
            return user;
        }
    }
}