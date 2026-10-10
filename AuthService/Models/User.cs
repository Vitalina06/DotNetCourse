using System;
using System.Text.Json.Serialization;

namespace AuthService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}