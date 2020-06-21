using EpoMaker.resources;
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
        public SQLiteConnection _connection;
        public List<string> courses = new List<string>();
        private string openedCourse;
        private void SetFileLoaded(bool value)
        {
            MENU_Course.IsEnabled = value;
            MENU_File_Close.IsEnabled = value;
            MENU_File_Save.IsEnabled = value;
            if (value)
            {
                _command.CommandText = string.Format(SQL_Statements.Get_All, @"TableList");
                SQLiteDataReader reader = _command.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(reader.GetString(0));
                }
                reader.Close();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            MENU_Course_Export.IsEnabled = false;
            
        }
        private void UpdateCourseBTNs()
        {
            int coursCount = courses.Count;
            int rowsCountNew =(int) Math.Round(Math.Sqrt(coursCount),0);
            int columnsCountNew = (int)Math.Ceiling(Math.Sqrt(coursCount));
            int rowsCountOld = MainGrid.RowDefinitions.Count - 2;
            int columnsCountOld = MainGrid.ColumnDefinitions.Count - 2;

            for (int i = 0; i < columnsCountNew - columnsCountOld; i++)
            {
                MainGrid.ColumnDefinitions.Insert(1, new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < rowsCountNew - rowsCountOld; i++)
            {
                MainGrid.RowDefinitions.Insert(2, new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
                    
            }

            int pos = -1;
            for (int i = 0; i < rowsCountNew; i++)
            {
                for (int j = 0; j < columnsCountNew; j++)
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

        private void RemoveFromGrid(Grid grid,int minRow,int minCol,int maxRow,int maxCol)
        {
            List<UIElement> toDelete = new List<UIElement>();
            foreach (UIElement o in grid.Children)
            {
                bool isInRowRange = Grid.GetRow(o) >= minRow && Grid.GetRow(o) <= maxRow;
                bool isInColRange = Grid.GetColumn(o) >= minCol && Grid.GetColumn(o) <= maxCol;
                if (isInColRange&&isInRowRange)
                {
                    toDelete.Add(o);                    
                }
            }
            foreach (UIElement uIElement in toDelete)
            {
                grid.Children.Remove(uIElement);
            }
            try
            {
                grid.RowDefinitions.RemoveRange(minRow, maxRow - minRow);
                grid.ColumnDefinitions.RemoveRange(minCol, maxCol - minCol);
            }
            catch (ArgumentOutOfRangeException) { }
        }
        private void ResetInnerGrid(Grid grid)
        {
            RemoveFromGrid(grid, 2, 1, grid.RowDefinitions.Count, grid.ColumnDefinitions.Count-1);
            MainGrid.ColumnDefinitions.Insert(1, new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        }
        public void OpenCourse(string course)
        {
            ResetInnerGrid(MainGrid);
            MainGrid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            MakeNotes makeNotesUC = new MakeNotes(_connection,course);
            if (makeNotesUC.successfullyLoaded)
            {
                MainGrid.Children.Add(makeNotesUC);
                Grid.SetColumn(makeNotesUC, 1);
                Grid.SetRow(makeNotesUC, 2);
                this.openedCourse = course;
                MENU_Course_Export.IsEnabled = true;
            }
            else
            {
                LoadCourseList();
            }
        }

        public void LoadCourseList()
        {
            ResetInnerGrid(MainGrid);
            MainGrid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            UpdateCourseBTNs();
            MENU_Course_Export.IsEnabled = false;
        }


    }
}
