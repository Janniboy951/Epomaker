using EpoMaker.resources;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1;

namespace EpoMaker
{
    partial class MainWindow
    {
        //**********************************File Menu**********************************************************
        private void MENU_File_New_Click(object sender, RoutedEventArgs e)
        {
            MENU_File_Close_Click(null, null);
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = langDE.FILEDIALOG_FileTypeName_EpoMakerFile + " (*.epmf)|*.epmf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + saveFileDialog.FileName + "; New = True; Compress = True; ");
                sqLiteConnection.Open();
                _connection = sqLiteConnection;
                _command = new SQLiteCommand(sqLiteConnection)
                {
                    
                    CommandText = SQL_Statements.Create_TableList_Table
                };
                _command.ExecuteNonQuery();
                SetFileLoaded(true);
                this.Title = langDE.WindowTitle + " - " + Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                UpdateCourseBTNs();
            }
        }

        private void MENU_File_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _command.Connection.Close();
            }
            catch (NullReferenceException)
            {
                
            }
            courses.Clear();
            ResetInnerGrid(MainGrid);
            SetFileLoaded(false);
            this.Title = langDE.WindowTitle;
        }

        private void MENU_File_Open_Click(object sender, RoutedEventArgs e)
        {
            MENU_File_Close_Click(null, null);
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = langDE.FILEDIALOG_FileTypeName_EpoMakerFile + " (*.epmf)|*.epmf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + openFileDialog.FileName + "; New = False; Compress = True; ");
                
                sqLiteConnection.Open();
                _command = new SQLiteCommand(sqLiteConnection);
                _connection = sqLiteConnection;
                SetFileLoaded(true);
                this.Title = langDE.WindowTitle+" - " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                UpdateCourseBTNs();
            }
        }

        private void MENU_File_Save_Click(object sender, RoutedEventArgs e)
        {

        }
        
        //**********************************Course Menu********************************************************
        private void MENU_Course_New_Click(object sender, RoutedEventArgs e)
        {
            string newCourseName = new InputBox(langDE.MESSAGE_EnterCourseName).ShowDialog();
            if (newCourseName != "")
            {


                if (!courses.Contains(newCourseName))
                {
                    _command.CommandText = string.Format(SQL_Statements.Create_Course_Table,newCourseName);
                    _command.ExecuteNonQuery();
                    _command.CommandText = string.Format(SQL_Statements.Insert_Course_To_TableList, newCourseName);
                    _command.ExecuteNonQuery();
                    _command.CommandText = string.Format(SQL_Statements.Create_Course_Data_Table, newCourseName);
                    _command.ExecuteNonQuery();
                    courses.Add(newCourseName);
                    UpdateCourseBTNs();
                }
                else
                {
                    MessageBox.Show(string.Format(langDE.MESSAGE_CourseAlreadyExists, newCourseName), langDE.WindowTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    
                }
            }
        }

        private void MENU_Course_Export_Click(object sender, RoutedEventArgs e)
        {
            ArrayList arrayList = new ArrayList();
            

            //Defines Header Row

            _command.CommandText = string.Format(SQL_Statements.Get_All_Lessons, openedCourse);
            SQLiteDataReader reader= _command.ExecuteReader();
            List<string> headerRow = new List<string>
            {
                langDE.TABLE_Cellheader_Surname,
                langDE.TABLE_Cellheader_Forename
            };
            while (reader.Read())
            {
                headerRow.Add(reader.GetString(0));
            }
            reader.Close();
            headerRow.Add("");
            headerRow.Add(langDE.TABLE_Cellheader_Average);
            arrayList.Add(headerRow.ToArray());

            _command.CommandText = string.Format(SQL_Statements.Get_Course_Persons, openedCourse);
            reader = _command.ExecuteReader();
            List<Person> persons = new List<Person>();
            while (reader.Read())
            {
                persons.Add(new Person { LastName=reader.GetString(0),PreName=reader.GetString(1),ID=reader.GetInt32(2)});
            }
            reader.Close();
            foreach (Person person in persons)
            {
                List<string> notes = new List<string>
                {
                    person.LastName,
                    person.PreName
                };
                _command.CommandText = string.Format(SQL_Statements.Get_All_Student_Grades, openedCourse,person.ID);
                reader = _command.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 0; i < headerRow.IndexOf(reader.GetString(0))-notes.Count; i++)
                    {
                        notes.Add("");
                    }
                    notes.Add(Math.Round(reader.GetDouble(1),1).ToString().Replace(",", "."));
                }
                reader.Close();
                int lengthDiff = (headerRow.Count - 1) - notes.Count;
                for (int i = 0; i < lengthDiff; i++)
                {
                    notes.Add("");
                }
                _command.CommandText = string.Format(SQL_Statements.Get_Single_Student_Grade_Average, openedCourse, person.ID);
                reader = _command.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                    notes.Add(Math.Round(reader.GetDouble(0), 1).ToString().Replace(",", "."));
                }
                else
                {
                    notes.Add("");
                }
                reader.Close();
                arrayList.Add(notes.ToArray());
            }

            new ExcelFile(arrayList);
        }
    }
}
