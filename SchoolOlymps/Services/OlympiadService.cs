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
    public class OlympiadService
    {
        private readonly AppDbContext _context;
        private readonly RatingService _ratingService;

        public OlympiadService()
        {
            _context = new AppDbContext();
            _ratingService = new RatingService();
        }

        public List<User> SearchStudents(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _context.Users
                    .Where(u => u.Role == UserRole.Student)
                    .Include(u => u.School)
                    .ToList();

            return _context.Users
                .Where(u => u.Role == UserRole.Student &&
                            (u.FullName.Contains(searchText) ||
                             u.Login.Contains(searchText) ||
                             (u.School != null && u.School.Name.Contains(searchText))))
                .Include(u => u.School)
                .ToList();
        }

        public List<Subject> GetAllSubjects()
        {
            return _context.Subjects.ToList();
        }

        public void CreateOlympiad(string name, DateTime date, int subjectId, double maxPoints, int organizerId, List<(int studentId, double points)> participants)
        {
            try
            {
                var olympiad = new Olympiad
                {
                    Name = name,
                    Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                    SubjectId = subjectId,
                    MaxPoints = maxPoints,
                    OrganizerId = organizerId,
                    Participants = participants.Select(p => new OlympiadParticipant
                    {
                        StudentId = p.studentId,
                        PointsEarned = p.points
                    }).ToList()
                };

                _context.Olympiads.Add(olympiad);
                _context.SaveChanges();

                // После сохранения обновить рейтинги
                _ratingService.UpdateRatingsForOlympiad(olympiad.Id);
            }
            catch (DbUpdateException ex)
            {
                // Показываем внутреннее исключение
                var inner = ex.InnerException;
                while (inner != null)
                {
                    throw new Exception($"Ошибка БД: {inner.Message}", inner);
                }
                throw;
            }
        }
    }
}
