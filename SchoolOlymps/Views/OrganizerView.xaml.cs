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
using SchoolOlymps.Services;

namespace SchoolOlymps.Views
{
    /// <summary>
    /// Логика взаимодействия для OrganizerView.xaml
    /// </summary>
    public partial class OrganizerView : UserControl
    {
        private readonly User _organizer;
        private readonly OlympiadService _olympiadService;
        private List<ParticipantViewModel> _participants = new List<ParticipantViewModel>();

        public OrganizerView(User organizer)
        {
            InitializeComponent();
            _organizer = organizer;
            _olympiadService = new OlympiadService();
            LoadSubjects();
            ParticipantsGrid.ItemsSource = _participants;
        }

        private void LoadSubjects()
        {
            SubjectCombo.ItemsSource = _olympiadService.GetAllSubjects();
            SubjectCombo.SelectedIndex = 0;
        }

        private void SearchStudent_Click(object sender, RoutedEventArgs e)
        {
            var students = _olympiadService.SearchStudents(StudentSearchBox.Text);
            SearchResultsList.ItemsSource = students;
        }

        private void AddParticipant_Click(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResultsList.SelectedItem is User student)
            {
                if (!_participants.Any(p => p.StudentId == student.Id))
                {
                    _participants.Add(new ParticipantViewModel
                    {
                        StudentId = student.Id,
                        StudentFullName = student.FullName,
                        SchoolName = student.School?.Name ?? "Нет школы",
                        Points = 0
                    });
                    ParticipantsGrid.ItemsSource = null;
                    ParticipantsGrid.ItemsSource = _participants;
                }
            }
        }

        private void RemoveParticipant_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var participant = btn?.DataContext as ParticipantViewModel;
            if (participant != null)
            {
                _participants.Remove(participant);
                ParticipantsGrid.ItemsSource = null;
                ParticipantsGrid.ItemsSource = _participants;
            }
        }

        private void SaveOlympiad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = OlympiadName.Text;
                DateTime date = OlympiadDate.SelectedDate ?? DateTime.Now;
                int subjectId = ((Subject)SubjectCombo.SelectedItem).Id;
                double maxPoints = double.Parse(MaxPointsBox.Text);

                // Просто берём данные из _participants
                var participantsList = _participants.Select(p => (p.StudentId, p.Points)).ToList();

                // Принудительно завершаем редактирование в DataGrid
                ParticipantsGrid.CommitEdit(DataGridEditingUnit.Row, true);

                // Проверяем баллы
                foreach (var p in _participants)
                {
                    if (p.Points < 0 || double.IsNaN(p.Points))
                        p.Points = 0;
                    if (p.Points > maxPoints)
                    {
                        MessageBox.Show($"Баллы {p.StudentFullName} не могут превышать максимальный балл ({maxPoints})", "Ошибка");
                        return;
                    }
                }

                _olympiadService.CreateOlympiad(name, date, subjectId, maxPoints, _organizer.Id, participantsList);
                MessageBox.Show("Олимпиада сохранена, рейтинги обновлены!", "Успех");

                OlympiadName.Text = "";
                _participants.Clear();
                ParticipantsGrid.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void ParticipantsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Проверяем, что редактируется столбец "Баллы"
            if (e.Column.Header.ToString() == "Баллы" && e.EditingElement is System.Windows.Controls.TextBox textBox)
            {
                string input = textBox.Text.Trim();
                double points = 0;

                // Если ввод не является числом или отрицательный, устанавливаем 0
                if (!double.TryParse(input, out points) || points < 0)
                    points = 0;

                // Обновляем модель участника
                var participant = e.Row.DataContext as ParticipantViewModel;
                if (participant != null)
                    participant.Points = points;

                // Отображаем скорректированное значение в ячейке
                textBox.Text = points.ToString();
            }
        }
    }
}
