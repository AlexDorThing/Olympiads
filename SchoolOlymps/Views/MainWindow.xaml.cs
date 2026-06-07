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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadView();
        }

        private void LoadView()
        {
            if (_currentUser == null)
                MainContent.Content = new GuestView();
            else if (_currentUser.Role == UserRole.Student)
                MainContent.Content = new StudentView(_currentUser);
            else if (_currentUser.Role == UserRole.Organizer)
                MainContent.Content = new OrganizerView(_currentUser);
            else if (_currentUser.Role == UserRole.Admin)
                MainContent.Content = new AdminView();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var auth = new AuthService();
            auth.Logout();
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}
