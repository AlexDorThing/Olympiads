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
using SchoolOlymps.Services;
using SchoolOlymps.Models;

namespace SchoolOlymps.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        private readonly AdminService _adminService;
        public AdminView()
        {
            InitializeComponent();
            _adminService = new AdminService();
            LoadUsers();
            LoadSchools();
            LoadOlympiads();
        }

        private void LoadUsers() => UsersGrid.ItemsSource = _adminService.GetAllUsers();
        private void LoadSchools() => SchoolsGrid.ItemsSource = _adminService.GetAllSchools();
        private void LoadOlympiads() => OlympiadsGrid.ItemsSource = _adminService.GetAllOlympiads();

        private void UserSearch_TextChanged(object sender, TextChangedEventArgs e) => UsersGrid.ItemsSource = _adminService.GetAllUsers(UserSearch.Text);
        // Обработчики для вкладки Пользователи
        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно оставить пустым, если не нужна логика при выборе строки
        }
        private void SchoolSearch_TextChanged(object sender, TextChangedEventArgs e) => SchoolsGrid.ItemsSource = _adminService.GetAllSchools(SchoolSearch.Text);

        private void EditSchool_Click(object sender, RoutedEventArgs e)
        {
            if (SchoolsGrid.SelectedItem is School school)
            {
                // Простой диалог редактирования школы
                var editWindow = new Window { Title = "Редактировать школу", Width = 400, Height = 200 };
                var stack = new StackPanel();
                var nameBox = new TextBox { Text = school.Name };
                var addressBox = new TextBox { Text = school.Address };
                var phoneBox = new TextBox { Text = school.PhoneNumber };
                var saveBtn = new Button { Content = "Сохранить" };
                saveBtn.Click += (s, ev) =>
                {
                    school.Name = nameBox.Text;
                    school.Address = addressBox.Text;
                    school.PhoneNumber = phoneBox.Text;
                    _adminService.UpdateSchool(school);
                    LoadSchools();
                    editWindow.Close();
                };
                stack.Children.Add(new TextBlock { Text = "Название" });
                stack.Children.Add(nameBox);
                stack.Children.Add(new TextBlock { Text = "Адрес" });
                stack.Children.Add(addressBox);
                stack.Children.Add(new TextBlock { Text = "Телефон" });
                stack.Children.Add(phoneBox);
                stack.Children.Add(saveBtn);
                editWindow.Content = stack;
                editWindow.ShowDialog();
            }
        }

        private void DeleteSchool_Click(object sender, RoutedEventArgs e)
        {
            if (SchoolsGrid.SelectedItem is School school)
            {
                if (MessageBox.Show("Удалить школу? Все связанные ученики потеряют привязку.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _adminService.DeleteSchool(school.Id);
                    LoadSchools();
                }
            }
        }
        private void OlympiadSearch_TextChanged(object sender, TextChangedEventArgs e) => OlympiadsGrid.ItemsSource = _adminService.GetAllOlympiads(OlympiadSearch.Text);

        private void EditOlympiad_Click(object sender, RoutedEventArgs e)
        {
            if (OlympiadsGrid.SelectedItem is Olympiad olympiad)
            {
                // Упрощённое редактирование – можно открыть диалог
                var editWindow = new Window { Title = "Редактировать олимпиаду", Width = 400, Height = 300 };
                var stack = new StackPanel();
                var nameBox = new TextBox { Text = olympiad.Name };
                var datePicker = new DatePicker { SelectedDate = olympiad.Date };
                var maxPointsBox = new TextBox { Text = olympiad.MaxPoints.ToString() };
                var saveBtn = new Button { Content = "Сохранить" };
                saveBtn.Click += (s, ev) =>
                {
                    olympiad.Name = nameBox.Text;
                    olympiad.Date = datePicker.SelectedDate ?? olympiad.Date;
                    olympiad.MaxPoints = double.Parse(maxPointsBox.Text);
                    _adminService.UpdateOlympiad(olympiad);
                    LoadOlympiads();
                    editWindow.Close();
                };
                stack.Children.Add(new TextBlock { Text = "Название" });
                stack.Children.Add(nameBox);
                stack.Children.Add(new TextBlock { Text = "Дата" });
                stack.Children.Add(datePicker);
                stack.Children.Add(new TextBlock { Text = "Макс. балл" });
                stack.Children.Add(maxPointsBox);
                stack.Children.Add(saveBtn);
                editWindow.Content = stack;
                editWindow.ShowDialog();
            }
        }

        private void DeleteOlympiad_Click(object sender, RoutedEventArgs e)
        {
            if (OlympiadsGrid.SelectedItem is Olympiad olympiad)
            {
                if (MessageBox.Show("Удалить олимпиаду? Рейтинги будут пересчитаны.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _adminService.DeleteOlympiad(olympiad.Id);
                    LoadOlympiads();
                }
            }
        }


        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User user)
            {
                // простой диалог редактирования
                var editWindow = new Window { Title = "Редактировать", Width = 300, Height = 200 };
                var stack = new StackPanel();
                var loginBox = new TextBox { Text = user.Login };
                var fullNameBox = new TextBox { Text = user.FullName };
                var saveBtn = new Button { Content = "Сохранить" };
                saveBtn.Click += (s, ev) =>
                {
                    user.Login = loginBox.Text;
                    user.FullName = fullNameBox.Text;
                    _adminService.UpdateUser(user);
                    LoadUsers();
                    editWindow.Close();
                };
                stack.Children.Add(new TextBlock { Text = "Логин" });
                stack.Children.Add(loginBox);
                stack.Children.Add(new TextBlock { Text = "ФИО" });
                stack.Children.Add(fullNameBox);
                stack.Children.Add(saveBtn);
                editWindow.Content = stack;
                editWindow.ShowDialog();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User user)
            {
                _adminService.DeleteUser(user.Id);
                LoadUsers();
            }
        }
    }
}
