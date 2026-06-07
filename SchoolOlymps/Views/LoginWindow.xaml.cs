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
using System.Windows.Shapes;
using SchoolOlymps.Services;
using SchoolOlymps.Models;

namespace SchoolOlymps.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly SchoolService _schoolService;
        private Window _registerWindow;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
            _schoolService = new SchoolService();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;
            if (_authService.Login(login, password))
            {
                var main = new MainWindow(_authService.CurrentUser!);
                main.Show();
                Close();
            }
            else
            {
                MessageText.Text = "Неверный логин или пароль";
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_registerWindow == null || !_registerWindow.IsVisible)
            {
                _registerWindow = new Window
                {
                    Title = "Регистрация",
                    Width = 400,
                    Height = 450,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };

                var stack = new StackPanel { Margin = new Thickness(20) };
                var loginBox = new System.Windows.Controls.TextBox { Margin = new Thickness(5), Tag = "Login" };
                var passBox = new System.Windows.Controls.PasswordBox { Margin = new Thickness(5) };
                var fullNameBox = new System.Windows.Controls.TextBox { Margin = new Thickness(5) };
                var roleCombo = new System.Windows.Controls.ComboBox { Margin = new Thickness(5) };
                roleCombo.Items.Add("Ученик");
                roleCombo.Items.Add("Организатор");
                roleCombo.SelectedIndex = 0;
                var schoolCombo = new System.Windows.Controls.ComboBox { Margin = new Thickness(5) };
                schoolCombo.DisplayMemberPath = "Name";
                schoolCombo.ItemsSource = _schoolService.SearchSchools("");
                var btn = new System.Windows.Controls.Button { Content = "Зарегистрироваться", Margin = new Thickness(5) };

                stack.Children.Add(new System.Windows.Controls.TextBlock { Text = "Логин" });
                stack.Children.Add(loginBox);
                stack.Children.Add(new System.Windows.Controls.TextBlock { Text = "Пароль" });
                stack.Children.Add(passBox);
                stack.Children.Add(new System.Windows.Controls.TextBlock { Text = "ФИО" });
                stack.Children.Add(fullNameBox);
                stack.Children.Add(new System.Windows.Controls.TextBlock { Text = "Роль" });
                stack.Children.Add(roleCombo);
                stack.Children.Add(new System.Windows.Controls.TextBlock { Text = "Школа (для ученика)" });
                stack.Children.Add(schoolCombo);
                stack.Children.Add(btn);

                btn.Click += (s, ev) =>
                {
                    string login = loginBox.Text;
                    string pass = passBox.Password;
                    string fullName = fullNameBox.Text;
                    var role = roleCombo.SelectedIndex == 0 ? UserRole.Student : UserRole.Organizer;
                    int? schoolId = null;
                    if (role == UserRole.Student && schoolCombo.SelectedItem is School sch)
                        schoolId = sch.Id;

                    if (_authService.Register(login, pass, fullName, role, schoolId))
                    {
                        MessageBox.Show("Регистрация успешна! Теперь войдите.", "Успех");
                        _registerWindow.Close();
                    }
                    else
                        MessageBox.Show("Логин уже существует", "Ошибка");
                };

                _registerWindow.Content = stack;
                _registerWindow.ShowDialog();
            }
        }
        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            // Открываем главное окно без авторизованного пользователя (null)
            var main = new MainWindow(null);
            main.Show();
            this.Close(); // Закрываем окно входа
        }
        private void AdminDemoBtn_Click(object sender, RoutedEventArgs e)
        {
            // Создаём фейкового администратора (или берём из БД, если есть)
            var adminUser = new User
            {
                Id = 1,
                Login = "admin_demo",
                FullName = "Демо-администратор",
                Role = UserRole.Admin,
                SchoolId = null
            };
            var main = new MainWindow(adminUser);
            main.Show();
            this.Close();
        }
    }
}
