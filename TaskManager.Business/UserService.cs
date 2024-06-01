using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Business
{
    public class UserService
    {

        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateUserAsync(string phoneNumber, string password)
        {
            var user = await _userRepository.GetUserByPhoneNumberAsync(phoneNumber);
            if (user != null && VerifyPassword(password, user.PASSWORD))
            {
                return user;
            }
            return null;
        }

        public async Task AddUserAsync(User user)
        {
            user.PASSWORD = HashPassword(user.PASSWORD);
            await _userRepository.AddUserAsync(user);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashInputPassword = HashPassword(inputPassword);
            return hashInputPassword == storedHash;
        }
    }
}
