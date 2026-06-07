using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOlymps.Data;
using SchoolOlymps.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolOlymps.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;
        private readonly RatingService _ratingService;

        public AdminService()
        {
            _context = new AppDbContext();
            _ratingService = new RatingService();
        }

        // Пользователи
        public List<User> GetAllUsers(string? search = null)
        {
            var query = _context.Users.Include(u => u.School).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.Login.Contains(search) || u.FullName.Contains(search));
            return query.ToList();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // Школы
        public List<School> GetAllSchools(string? search = null)
        {
            var query = _context.Schools.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.Name.Contains(search));
            return query.ToList();
        }

        public void UpdateSchool(School school)
        {
            _context.Schools.Update(school);
            _context.SaveChanges();
        }

        public void DeleteSchool(int schoolId)
        {
            var school = _context.Schools.Find(schoolId);
            if (school != null)
            {
                _context.Schools.Remove(school);
                _context.SaveChanges();
            }
        }

        // Олимпиады (удаление с пересчётом)
        public List<Olympiad> GetAllOlympiads(string? search = null)
        {
            var query = _context.Olympiads.Include(o => o.Subject).Include(o => o.Organizer).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o => o.Name.Contains(search) || o.Subject.Name.Contains(search));
            return query.ToList();
        }

        public void UpdateOlympiad(Olympiad olympiad)
        {
            _context.Olympiads.Update(olympiad);
            _context.SaveChanges();
            // При изменении олимпиады нужно пересчитать рейтинги, но для упрощения - пересчёт всех затронутых
            var pairs = _context.OlympiadParticipants
                .Where(p => p.OlympiadId == olympiad.Id && p.Student.SchoolId != null)
                .Select(p => new { p.Student.SchoolId, olympiad.SubjectId })
                .Distinct();
            foreach (var p in pairs)
                _ratingService.RecalcAllRatingsAfterOlympiadDeletion(olympiad.Id); // пересчёт после удаления старой
            _context.SaveChanges();
        }

        public void DeleteOlympiad(int olympiadId)
        {
            var olympiad = _context.Olympiads
                .Include(o => o.Participants)
                .FirstOrDefault(o => o.Id == olympiadId);
            if (olympiad != null)
            {
                _context.OlympiadParticipants.RemoveRange(olympiad.Participants);
                _context.Olympiads.Remove(olympiad);
                _context.SaveChanges();

                // Пересчитать рейтинги для всех школ и предметов, которые были в этой олимпиаде
                _ratingService.RecalcAllRatingsAfterOlympiadDeletion(olympiadId);
            }
        }
    }
}
