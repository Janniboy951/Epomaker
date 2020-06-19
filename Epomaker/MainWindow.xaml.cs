using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace EpoMaker
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SQLiteCommand _command;
        public List<string> courses = new List<string>();
        bool _fileLoaded
        {
            get { return fileLoaded; }
            set
            {
                fileLoaded = value;
                MENU_Course.IsEnabled = value;
                MENU_Close.IsEnabled = value;
                MENU_Save.IsEnabled = value;
                if (value)
                {
                    _command.CommandText = @"SELECT * FROM TableList";
                    SQLiteDataReader reader= _command.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add($"{reader.GetString(0)}");
                    }
                }
            }
        }
        private bool fileLoaded = false;
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine(MainGrid.RowDefinitions.Count);
        }
        private void CreateCourseBTN()
        {
            int coursCount = courses.Count;
            int rows =(int) Math.Round(Math.Sqrt(coursCount),0);
            int cols = (int)Math.Ceiling(Math.Sqrt(coursCount));
            for (int i = 0; i < cols-(MainGrid.ColumnDefinitions.Count-2); i++)
            {
                MainGrid.ColumnDefinitions.Insert(1, new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }
            for (int i = 0; i < rows-(MainGrid.RowDefinitions.Count-3); i++)
            {
                MainGrid.RowDefinitions.Insert(2, new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }
            int pos = -2;
            for (int i = 0; i < rows; i++)
            {
                pos++;
                for (int j = 0; j < cols; j++)
                {
                    pos++;
                    if (pos < courses.Count) { 
                        CourseBTN courseBTN = new CourseBTN(courses[pos]);
                        Grid.SetRow(courseBTN, i+2);
                        Grid.SetColumn(courseBTN, j+1);
                        MainGrid.Children.Add(courseBTN);
                    }
                    else
                    {

                        return;
                    }
                    

                }
            }
        }


    }
}
