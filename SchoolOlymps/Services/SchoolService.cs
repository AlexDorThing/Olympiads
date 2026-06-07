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
    public class SchoolService
    {
        private readonly AppDbContext _context;

        public SchoolService()
        {
            _context = new AppDbContext();
        }

        public List<School> SearchSchools(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _context.Schools.ToList();

            return _context.Schools
                .Where(s => s.Name.Contains(searchText))
                .ToList();
        }

        public List<SchoolSubjectRating> GetSchoolRatings(int schoolId)
        {
            return _context.SchoolSubjectRatings
                .Include(r => r.Subject)
                .Where(r => r.SchoolId == schoolId)
                .ToList();
        }

        public double? GetOverallRating(int schoolId)
        {
            var ratings = _context.SchoolSubjectRatings
                .Where(r => r.SchoolId == schoolId)
                .Select(r => r.Rating)
                .ToList();
            if (ratings.Count == 0) return null;
            return ratings.Average();
        }
    }
}
