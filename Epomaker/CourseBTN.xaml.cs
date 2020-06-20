using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
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

        private void CourseBTN_MENU_EDIT_Click(object sender, RoutedEventArgs e)
        {
            
            EditUsers editUsers = new EditUsers(BTNCourse.Content.ToString());
            editUsers.ShowDialog();
            
        }
        private string oldTableName;

        private void CourseBTN_MENU_RENAME_Click(object sender, RoutedEventArgs e)
        {
            oldTableName = (string)BTNCourse.Content;


            TextBox textbox= new TextBox
            {
                Text= (string)BTNCourse.Content
            };
            
            BTNCourse.Content = textbox;
            textbox.Focus();
            textbox.SelectAll();
            Keyboard.Focus(textbox);
            textbox.Focusable = true;
            textbox.KeyDown += TextboxRenameKeyPressed;
        }

        private void TextboxRenameKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!((MainWindow)Application.Current.MainWindow).courses.Contains(((TextBox)sender).Text))
                {
                    BTNCourse.Content = ((TextBox)sender).Text;
                    SQLiteCommand sqlCommand = new SQLiteCommand(((MainWindow)Application.Current.MainWindow)._connection);
                    sqlCommand.CommandText = @"ALTER TABLE '" + oldTableName + "' RENAME TO '" + ((TextBox)sender).Text + "'";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = "UPDATE TableList SET Name = '" + ((TextBox)sender).Text + "' WHERE Name = '" + oldTableName + "' ";
                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("Der Kurs " + ((TextBox)sender).Text + " existiert bereits!", "Eponoten Maker", MessageBoxButton.OK, MessageBoxImage.Error);
                    BTNCourse.Content = oldTableName;
                }
            }
            else if(e.Key == Key.Escape)
            {
                BTNCourse.Content = oldTableName;
            }
        }
        private void CourseBTN_MENU_DELETE_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNCourse_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).OpenCourse((string)BTNCourse.Content);
        }
    }
}
