﻿using Microsoft.Win32;
using System;
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
        private void MENU_New_Click(object sender, RoutedEventArgs e)
        {
            MENU_Close_Click(null, null);
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Epomaker Datei (*.epmf)|*.epmf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + saveFileDialog.FileName + "; New = True; Compress = True; ");
                sqLiteConnection.Open();
                _connection = sqLiteConnection;
                _command = new SQLiteCommand(sqLiteConnection)
                {
                    CommandText = @"CREATE TABLE 'TableList' ('Name'TEXT);"
                };
                _command.ExecuteNonQuery();
                _fileLoaded = true;
                this.Title = "Eponoten Maker - "+Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                UpdateCourseBTNs();
            }
        }

        private void MENU_Close_Click(object sender, RoutedEventArgs e)
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
            _fileLoaded = false;
            this.Title = "Eponoten Maker";
        }

        private void MENU_Open_Click(object sender, RoutedEventArgs e)
        {
            MENU_Close_Click(null, null);
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Epomaker Datei (*.epmf)|*.epmf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SQLiteConnection sqLiteConnection = new SQLiteConnection("Data Source=" + openFileDialog.FileName + "; New = False; Compress = True; ");
                
                sqLiteConnection.Open();
                _command = new SQLiteCommand(sqLiteConnection);
                _connection = sqLiteConnection;
                _fileLoaded = true;
                this.Title = "Eponoten Maker - " + Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                UpdateCourseBTNs();
            }
        }

        private void MENU_Save_Click(object sender, RoutedEventArgs e)
        {

        }
        
        //**********************************Course Menu********************************************************
        private void MENU_Course_New_Click(object sender, RoutedEventArgs e)
        {
            string newCoursName = new InputBox("Namen des neuen Kurs eingeben:").ShowDialog();
            if (newCoursName != "")
            {


                if (!courses.Contains(newCoursName))
                {
                    _command.CommandText = @"CREATE TABLE '" + newCoursName + @"' ('schuelerID'INTEGER,'Vorname'TEXT,'Nachname'TEXT,PRIMARY KEY('schuelerID'))";
                    _command.ExecuteNonQuery();
                    _command.CommandText = @"Insert INTO TableList (Name) Values ('"+newCoursName+"')";
                    _command.ExecuteNonQuery();
                    courses.Add(newCoursName);
                    UpdateCourseBTNs();
                }
                else
                {
                    MessageBox.Show("Kurs existiert bereits!", "EpoMaker", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    
                }
            }
        }
        
    }
}
