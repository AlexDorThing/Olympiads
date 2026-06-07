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
    public class RatingService
    {
        private readonly AppDbContext _context;

        public RatingService()
        {
            _context = new AppDbContext();
        }

        // Вызывать после сохранения новой олимпиады
        public void UpdateRatingsForOlympiad(int olympiadId)
        {
            var olympiad = _context.Olympiads
                .Include(o => o.Participants)
                    .ThenInclude(p => p.Student)
                .First(o => o.Id == olympiadId);
            var subjectId = olympiad.SubjectId;
            var maxPoints = olympiad.MaxPoints;

            // Группируем участников по школе
            var groups = olympiad.Participants
                .Where(p => p.Student.SchoolId.HasValue)
                .GroupBy(p => p.Student.SchoolId.Value);

            foreach (var group in groups)
            {
                int schoolId = group.Key;
                double sumK = group.Sum(p => p.PointsEarned / maxPoints);
                double r_olymp = sumK / group.Count();

                // Получаем текущий рейтинг школы по предмету
                var rating = _context.SchoolSubjectRatings
                    .FirstOrDefault(r => r.SchoolId == schoolId && r.SubjectId == subjectId);

                if (rating == null)
                {
                    rating = new SchoolSubjectRating
                    {
                        SchoolId = schoolId,
                        SubjectId = subjectId,
                        Rating = r_olymp
                    };
                    _context.SchoolSubjectRatings.Add(rating);
                }
                else
                {
                    // Пересчитываем среднее арифметическое по всем олимпиадам (включая новую)
                    RecalculateAverageForSchoolSubject(schoolId, subjectId);
                }
            }
            _context.SaveChanges();
        }

        // Пересчёт рейтинга для конкретной школы и предмета с нуля (по всем олимпиадам)
        private void RecalculateAverageForSchoolSubject(int schoolId, int subjectId)
        {
            // Все олимпиады по этому предмету, где участвовала данная школа
            var olympiads = _context.Olympiads
                .Include(o => o.Participants)
                    .ThenInclude(p => p.Student)
                .Where(o => o.SubjectId == subjectId)
                .ToList();

            var r_olympList = new List<double>();

            foreach (var olympiad in olympiads)
            {
                var participantsFromSchool = olympiad.Participants
                    .Where(p => p.Student.SchoolId == schoolId);
                if (!participantsFromSchool.Any()) continue;

                double sumK = participantsFromSchool.Sum(p => p.PointsEarned / olympiad.MaxPoints);
                double r_olymp = sumK / participantsFromSchool.Count();
                r_olympList.Add(r_olymp);
            }

            double newRating = r_olympList.Count > 0 ? r_olympList.Average() : 0;

            var rating = _context.SchoolSubjectRatings
                .FirstOrDefault(r => r.SchoolId == schoolId && r.SubjectId == subjectId);
            if (rating != null)
            {
                if (r_olympList.Count == 0)
                    _context.SchoolSubjectRatings.Remove(rating);
                else
                    rating.Rating = newRating;
            }
            else if (r_olympList.Count > 0)
            {
                _context.SchoolSubjectRatings.Add(new SchoolSubjectRating
                {
                    SchoolId = schoolId,
                    SubjectId = subjectId,
                    Rating = newRating
                });
            }
        }

        // При удалении олимпиады вызывать для всех затронутых пар (школа, предмет)
        public void RecalcAllRatingsAfterOlympiadDeletion(int olympiadId)
        {
            // Найдём все школы и предметы, которые были в этой олимпиаде
            var olympiad = _context.Olympiads
                .Include(o => o.Participants)
                    .ThenInclude(p => p.Student)
                .First(o => o.Id == olympiadId);

            var affectedPairs = olympiad.Participants
                .Where(p => p.Student.SchoolId.HasValue)
                .Select(p => new { SchoolId = p.Student.SchoolId.Value, SubjectId = olympiad.SubjectId })
                .Distinct()
                .ToList();

            foreach (var pair in affectedPairs)
            {
                RecalculateAverageForSchoolSubject(pair.SchoolId, pair.SubjectId);
            }
            _context.SaveChanges();
        }
    }
}
