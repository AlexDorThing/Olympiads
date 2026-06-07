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

namespace SchoolOlymps.Views
{
    /// <summary>
    /// Логика взаимодействия для GuestView.xaml
    /// </summary>
    public partial class GuestView : UserControl
    {
        private readonly SchoolService _schoolService;

        public GuestView()
        {
            InitializeComponent();
            _schoolService = new SchoolService();
            LoadSchools();
        }

        private void LoadSchools()
        {
            SchoolsGrid.ItemsSource = _schoolService.SearchSchools("");
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SchoolsGrid.ItemsSource = _schoolService.SearchSchools(SearchBox.Text);
        }

        private void SchoolsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SchoolsGrid.SelectedItem is Models.School school)
            {
                var ratings = _schoolService.GetSchoolRatings(school.Id);
                RatingsGrid.ItemsSource = ratings;
            }
        }
    }
}
