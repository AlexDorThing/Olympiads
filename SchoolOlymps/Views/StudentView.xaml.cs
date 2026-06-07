using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SchoolOlymps.Models;
using SchoolOlymps.Data;
using Microsoft.EntityFrameworkCore;

namespace SchoolOlymps.Views
{
    /// <summary>
    /// Логика взаимодействия для StudentView.xaml
    /// </summary>
    public partial class StudentView : UserControl
    {
        private readonly User _student;

        public StudentView(User student)
        {
            InitializeComponent();
            _student = student;
            LoadResults();
        }

        private void LoadResults()
        {
            using var db = new AppDbContext();
            var results = db.OlympiadParticipants
                .Where(p => p.StudentId == _student.Id)
                .Include(p => p.Olympiad)
                    .ThenInclude(o => o.Subject)
                .Select(p => new
                {
                    Олимпиада = p.Olympiad.Name,
                    Предмет = p.Olympiad.Subject.Name,
                    Дата = p.Olympiad.Date,
                    Баллы = p.PointsEarned,
                    МаксБалл = p.Olympiad.MaxPoints,
                    Процент = (p.PointsEarned / p.Olympiad.MaxPoints) * 100
                })
                .ToList();
            ResultsGrid.ItemsSource = results;
        }
    }
}
