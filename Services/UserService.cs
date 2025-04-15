using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Week5.Models;
using Microsoft.AspNetCore.Hosting;

namespace Week5.Services
{
    public class UserService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _usersFilePath;

        public UserService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _usersFilePath = Path.Combine(_environment.WebRootPath, "data", "users.json");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<User>();
            }

            string jsonContent = await File.ReadAllTextAsync(_usersFilePath);
            return JsonSerializer.Deserialize<List<User>>(jsonContent) ?? new List<User>();
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u => 
                u.Username == username && 
                u.Password == password && 
                u.IsActive);
        }
    }
}