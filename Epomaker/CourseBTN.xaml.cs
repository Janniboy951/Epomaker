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
using WpfApp1;

namespace EpoMaker
{
    /// <summary>
    /// Interaktionslogik für CourseBTN.xaml
    /// </summary>
    public partial class CourseBTN : UserControl
    {
        public CourseBTN(string courseName)
        {
            InitializeComponent();
            BTNCourse.Content = courseName;
        }
        public CourseBTN()
        {
            InitializeComponent();
            BTNCourse.Content = "9a";
        }

        private void CourseBTN_MENU_EDIT_Click(object sender, RoutedEventArgs e)
        {
            EditUsers editUsers = new EditUsers(BTNCourse.Content.ToString());
            editUsers.Show();
            
        }
    }
}
