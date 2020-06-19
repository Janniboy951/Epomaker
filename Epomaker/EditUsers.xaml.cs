using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SQLite;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EpoMaker
{
    /// <summary>
    /// Interaktionslogik für EditUsers.xaml
    /// </summary>
    public partial class EditUsers : Window
    {
        public EditUsers(string course)
        {
            InitializeComponent();
            SQLiteCommand sqlite = ((MainWindow)Application.Current.MainWindow)._command;
            sqlite.CommandText = @"SELECT Count(*) From " + course;
            SQLiteDataReader reader = sqlite.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)}");
            }
        }
    }
}
