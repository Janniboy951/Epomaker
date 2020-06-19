using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EpoMaker
{
    public class InputBox
    {
        readonly Window Box = new Window();//window for the inputbox
        readonly FontFamily font = new FontFamily("Arial");//font for the whole inputbox
        readonly int FontSize = 14;//fontsize for the input
        readonly StackPanel sp1 = new StackPanel();// items container
        readonly string title = "EpoMaker";//title as heading
        readonly string boxcontent;//title
        readonly string defaulttext = "";//default textbox content
        readonly string errormessage = "Fehler";//error messagebox content
        readonly string errortitle = "Error";//error messagebox heading title
        readonly string okbuttontext = "Kurs hinzufügen";//Ok button content
        readonly Brush BoxBackgroundColor = Brushes.LightGray;// Window Background
        readonly Brush InputBackgroundColor = Brushes.Ivory;// Textbox Background
        bool clickedOk = false;
        readonly TextBox input = new TextBox();
        readonly Button ok = new Button();
        bool inputreset = false;


        public InputBox(string content)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            Windowdef();
        }

        public InputBox(string content, string Htitle, string DefaultText)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            try
            {
                defaulttext = DefaultText;
            }
            catch
            {
                DefaultText = "Error!";
            }
            Windowdef();
        }

        public InputBox(string content, string Htitle, string Font, int Fontsize)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                font = new FontFamily(Font);
            }
            catch { font = new FontFamily("Tahoma"); }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            if (Fontsize >= 1)
                FontSize = Fontsize;
            Windowdef();
        }

        private void Windowdef()// window building - check only for window size
        {
            Box.Height = 150;// Box Height
            Box.Width = 350;// Box Width
            Box.Background = BoxBackgroundColor;
            Box.Title = title;
            Box.Content = sp1;
            Box.Closing += Box_Closing;
            Box.ResizeMode = ResizeMode.NoResize;

            TextBlock content = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Background = null,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 10, 0, 0),
                Text = boxcontent,
                FontFamily = font,
                FontSize = FontSize
            };
            sp1.Children.Add(content);

            input.Background = InputBackgroundColor;
            input.FontFamily = font;
            input.FontSize = FontSize;
            input.HorizontalAlignment = HorizontalAlignment.Left;
            input.Margin = new Thickness(10, 10, 10, 0);
            input.Text = defaulttext;
            input.Width = 312;
            input.MouseEnter += Input_MouseDown;
            input.KeyDown += Input_KeyDown;

            sp1.Children.Add(input);

            ok.Width = 120;
            ok.Height = 30;
            ok.Click += Ok_Click;
            ok.Content = okbuttontext;
            ok.Margin = new Thickness(0, 10, 10, 0);
            ok.HorizontalAlignment = HorizontalAlignment.Right;

            WrapPanel gboxContent = new WrapPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };

            sp1.Children.Add(gboxContent);
            gboxContent.Children.Add(ok);

            input.Focus();
        }

        void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //validation
        }

        private void Input_MouseDown(object sender, MouseEventArgs e)
        {
            if ((sender as TextBox).Text == defaulttext && inputreset == false)
            {
                (sender as TextBox).Text = null;
                inputreset = true;
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && clickedOk == false)
            {
                e.Handled = true;
                Ok_Click(input, null);
            }

            if (e.Key == Key.Escape)
            {
                Cancel_Click(input, null);
            }
        }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            clickedOk = true;
            if (input.Text == defaulttext || input.Text == "")
                MessageBox.Show(errormessage, errortitle, MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                Box.Close();
            }
            clickedOk = false;
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Box.Close();
        }

        public string ShowDialog()
        {
            Box.ShowDialog();
            return input.Text;
        }
    }
}
