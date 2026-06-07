using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOlymps.Data;
using SchoolOlymps.Models;
using BCrypt.Net;

namespace SchoolOlymps.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        public User? CurrentUser { get; private set; }

        public AuthService()
        {
            _context = new AppDbContext();
        }

        public bool Register(string login, string password, string fullName, UserRole role, int? schoolId = null)
        {
            if (_context.Users.Any(u => u.Login == login))
                return false;

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Login = login,
                PasswordHash = hash,
                FullName = fullName,
                Role = role,
                SchoolId = schoolId
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public bool Login(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == login);
            if (user == null) return false;
            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                CurrentUser = user;
                return true;
            }
            
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
