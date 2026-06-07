using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolOlymps.Models;

namespace SchoolOlymps.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Olympiad> Olympiads { get; set; }
        public DbSet<OlympiadParticipant> OlympiadParticipants { get; set; }
        public DbSet<SchoolSubjectRating> SchoolSubjectRatings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Host=localhost;Database=SchoolOlympiadDB;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>().HasData(
    new School { Id = 1, Name = "Школа №1", Address = "ул. Ленина, 10", PhoneNumber = "+7 (123) 456-78-90" },
    new School { Id = 2, Name = "Школа №2", Address = "ул. Пушкина, 5", PhoneNumber = "+7 (123) 456-78-91" }
);
            // Уникальный логин
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Уникальная пара (SchoolId, SubjectId) в рейтингах
            modelBuilder.Entity<SchoolSubjectRating>()
                .HasIndex(r => new { r.SchoolId, r.SubjectId })
                .IsUnique();

            // Добавим несколько предметов по умолчанию (можно выполнить через миграцию)
            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Математика" },
                new Subject { Id = 2, Name = "Физика" },
                new Subject { Id = 3, Name = "Информатика" },
                new Subject { Id = 4, Name = "Русский язык" },
                new Subject { Id = 5, Name = "Английский язык" }
            );
        }
    }
}
