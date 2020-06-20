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
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
namespace EpoMaker
{
    /// <summary>
    /// Interaktionslogik für EditUsers.xaml
    /// </summary>
    public partial class EditUsers : Window
    {
        private SQLiteCommand sqlite;
        private string course;
        private List<Person> personList = new List<Person>();
        public EditUsers(string course)
        {
            this.course = course;
            InitializeComponent();
            this.Closed += ClosedWindow;
            
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = personList;


            SQLiteConnection sqlitecon = ((MainWindow)Application.Current.MainWindow)._connection;
            sqlite = new SQLiteCommand(sqlitecon);
            SQLiteCommand sqlite1 = new SQLiteCommand(sqlitecon)
            {
                CommandText = @"SELECT * From '" + course + "'"
            };
            SQLiteDataReader reader = sqlite1.ExecuteReader();
            while (reader.Read())
            {
                personList.Add(new Person { ID = reader.GetInt32(0), PreName = reader.GetString(1), LastName = reader.GetString(2) });
            }
        }
        private void ClosedWindow(object sender, EventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow).Show();
            
        }
        private void BTNSave_Click(object sender, RoutedEventArgs e)
        {
            sqlite.CommandText = @"DELETE FROM '" + course + "'";
            sqlite.ExecuteNonQuery();
            foreach (Person person in personList)
            {
                sqlite.CommandText = @"INSERT INTO '" + course + "'(schuelerID,Vorname,Nachname) VALUES ("+person.ID+",'"+person.PreName+"','"+person.LastName+"')";
                sqlite.ExecuteNonQuery();
            }
            this.Close();

        }
        private void BTNDiscard_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
    public class Person
    {
        public int ID { get; set; }
        public string PreName { get; set; }
        public string LastName { get; set; }
    }
}
