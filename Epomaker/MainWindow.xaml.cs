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
        private void UpdateCourseBTNs()
        {
            int coursCount = courses.Count;
            int rowsCountNew =(int) Math.Round(Math.Sqrt(coursCount),0);
            int columnsCountNew = (int)Math.Ceiling(Math.Sqrt(coursCount));
            int rowsCountOld = MainGrid.RowDefinitions.Count - 2;
            int columnsCountOld = MainGrid.ColumnDefinitions.Count - 2;
            if (columnsCountNew - columnsCountOld > 0)
            {
                for (int i = 0; i < columnsCountNew - columnsCountOld; i++)
                {
                    MainGrid.ColumnDefinitions.Insert(1, new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }
            }
            else if (columnsCountNew - columnsCountOld < 0)
            {
                for (int i = 0; i <= columnsCountOld - columnsCountNew; i++)
                {
                    //MainGrid.ColumnDefinitions.RemoveAt(1);
                }
            }
            if (rowsCountNew - rowsCountOld>0)
            {
                for (int i = 0; i < rowsCountNew - rowsCountOld; i++)
                {
                    MainGrid.RowDefinitions.Insert(2, new RowDefinition
                    {
                        Height = new GridLength(1, GridUnitType.Star)
                    });
                    MainGrid.Children.Remove(new CourseBTN());
                }
            }
            else if (rowsCountNew - rowsCountOld < 0)
            {
                for (int i = 0; i < rowsCountOld - rowsCountNew; i++)
                {
                    MainGrid.RowDefinitions.RemoveAt(2);
                }
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
            RemoveFromGrid(grid, 2, 2, grid.RowDefinitions.Count, grid.ColumnDefinitions.Count);
        }
    }
}
