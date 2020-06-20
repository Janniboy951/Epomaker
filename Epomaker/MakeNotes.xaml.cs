using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EpoMaker
{
    /// <summary>
    /// Interaktionslogik für MakeNotes.xaml
    /// </summary>
    public partial class MakeNotes : UserControl
    {
        public bool IsDoublehour
        {
            get { return _IsDoublehour; }
            set
            {
                _IsDoublehour = value;
            }
        }
        public bool successfullyLoaded;
        private bool _IsDoublehour=true;
        private SQLiteConnection connection;
        private string course;
        private List<Person> courseMembers = new List<Person>();
        private int currentMember = 0;
        public MakeNotes(SQLiteConnection con,string course)
        {
            InitializeComponent();
            DatePickBox.SelectedDate = DateTime.Today;
            this.connection = con;
            this.course = course; 
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = @"SELECT * FROM '"+course+"'";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Person person = new Person
                { 
                    ID=reader.GetInt32(0),
                    PreName=reader.GetString(1),
                    LastName=reader.GetString(2)
                };
                courseMembers.Add(person);
            }
            reader.Close();
            if (courseMembers.Count == 0)
            {
                MessageBox.Show("Bitte füge erst ein Paar Mitglieder hinzu!", "Eponoten Maker", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                successfullyLoaded = false;
                return;
                
            }

            command.CommandText = @"SELECT CurrentPos FROM TableList WHERE Name='"+course+"'";
            reader = command.ExecuteReader();
            reader.Read();
            int dbPos = 0;
            if (!reader.IsDBNull(0))
            {
                dbPos = reader.GetInt32(0);
            }
            
            reader.Close();
            if (dbPos != 0)
            {
                MessageBoxResult boxResult= MessageBox.Show("Möchtest du da weitermachen wo du aufgehört hast?", "Eponoten Maker", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (boxResult == MessageBoxResult.Yes)
                {
                    currentMember = dbPos;
                }
                command.CommandText = @"UPDATE 'TableList' SET CurrentPos=0 WHERE Name='" + course + "'";
                command.ExecuteNonQuery();
            }
            reader.Close();
            NameField.Content = courseMembers[currentMember].LastName + ", " + courseMembers[currentMember].PreName;
            successfullyLoaded = true;
        }

        private void BTNCancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).LoadCourseList();
            if (currentMember < courseMembers.Count)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = @"UPDATE 'TableList' SET CurrentPos="+currentMember+" WHERE Name='"+course+"'";
                command.ExecuteNonQuery();
            }
        }

        private void BTNSKip_Click(object sender, RoutedEventArgs e)
        {
            NoteSlider.Value = 8;
            hideBorder.Visibility = Visibility.Hidden;
            BTNMissing.IsChecked = false;
            currentMember++;
            if (currentMember < courseMembers.Count)
            {
                NameField.Content = courseMembers[currentMember].LastName + ", " + courseMembers[currentMember].PreName;
            }
            else
            {
                BTNCancel_Click(null, null);
            }
        }

        private void BTNSaveNext_Click(object sender, RoutedEventArgs e)
        {
            if (DoubleHour.IsChecked == false && SingleHour.IsChecked == false)
            {

                DoubleHour.Foreground = Brushes.Red;
                SingleHour.Foreground = Brushes.Red;
            }
            else 
            {
                DoubleHour.Foreground = Brushes.Black;
                SingleHour.Foreground = Brushes.Black;
                SQLiteCommand command = new SQLiteCommand(connection);
                int id = courseMembers[currentMember].ID;
                command.CommandText = @"INSERT INTO '" + course + "-Data'('schelerId','Note','Stunde','Fehlt','Doppelstunde') VALUES ("+id+",'"+ NoteSlider.Value.ToString().Replace(',','.') + "','"+ 
                    ((DateTime)DatePickBox.SelectedDate).ToString("d")+"','"+ BTNMissing.IsChecked + "','"+ DoubleHour.IsChecked + "');";
                command.ExecuteNonQuery();
                NoteSlider.Value = 8;
                hideBorder.Visibility = Visibility.Hidden;
                BTNMissing.IsChecked = false;
                currentMember++;
                if(currentMember< courseMembers.Count)
                {
                    NameField.Content = courseMembers[currentMember].LastName + ", " + courseMembers[currentMember].PreName;
                }
                else
                {
                    BTNCancel_Click(null, null);
                }
                
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {

            if (BTNMissing.IsChecked==true)
            {
                hideBorder.Visibility = Visibility.Visible;
            }
            else
            {
                hideBorder.Visibility = Visibility.Hidden;
            }
            
        }
    }
}
