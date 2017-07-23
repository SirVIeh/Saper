using System;
using System.Collections.Generic;
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

namespace Saper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum Level { Easy, Medium, Hard};
        public static Level Difficulty = Level.Easy;
        public List<Button> Buttons = new List<Button>();
        public bool[,] MineCollection;
        public int ColumnCount = 0;
        Random randomMinePlace = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }

        public void DrawLevel()
        {
            MainGrid.Children.Clear();
            if (MainGrid.RowDefinitions.Count > 0 && MainGrid.ColumnDefinitions.Count > 0)
            {
                MainGrid.RowDefinitions.RemoveRange(0, MainGrid.RowDefinitions.Count - 1);
                MainGrid.ColumnDefinitions.RemoveRange(0, MainGrid.ColumnDefinitions.Count - 1);
            }
            ColumnCount = 0;
            switch (Difficulty)
            {
                case Level.Easy:
                    ColumnCount = 10;
                    MineCollection = new bool[10,10];
                    for (int i = 0; i < 20; i++)
                    {
                        int r1 = randomMinePlace.Next(DateTime.Now.Millisecond%ColumnCount);
                        int r2 = randomMinePlace.Next(DateTime.Now.Millisecond%ColumnCount);
                        while (AddMine(ref r1, ref r2))
                        {
                            r1 = randomMinePlace.Next((DateTime.Now.Millisecond + i) % ColumnCount);
                            r2 = randomMinePlace.Next((DateTime.Now.Millisecond + i) % ColumnCount);
                        }
                    }
                    break;
                case Level.Medium:
                    ColumnCount = 20;
                    break;
                case Level.Hard:
                    ColumnCount = 30;
                    break;
            }

            for (int i = 0; i < ColumnCount; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(column);
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                MainGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    Button button = new Button();
                    button.Name = string.Format("B_{0}{1}", i, j);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    button.Click += button_Click;
                    button.MouseUp += button_MouseUp;
                    button.Background = Brushes.Black;
                    button.Foreground = Brushes.White;
                    Buttons.Add(button);
                    MainGrid.Children.Add(button);
                }
            }
        }

        void button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Button b = (Button)sender;
            if (e.ChangedButton == MouseButton.Right && b.Content == null && b.Background != Brushes.White)
            {
                b.Content = "Flaged";
            }
            else if (e.ChangedButton == MouseButton.Right && b.Content == "Flaged")
            {
                b.Content = null;
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            var hasMine =
                MineCollection[Convert.ToInt32(b.Name.Substring(2, 1)), Convert.ToInt32(b.Name.Substring(3, 1))];
            if (b.Content != "Flaged")
            {
                if (hasMine)
                {
                    b.Content = "*";
                }
                else
                {
                    b.Background = Brushes.White;
                }
            }
            //MessageBox.Show(b.Name + "Have i mine ? = " + hasMine);
        }

        private void Difficulty_onClick(object sender, RoutedEventArgs e)
        {
            var difChoice = (MenuItem) sender;
            string difName = difChoice.Header.ToString().Substring(1, difChoice.Header.ToString().Count()-1);

            switch (difName)
            {
                case "Easy":
                    Difficulty = Level.Easy;
                    break;
                case "Medium":
                    Difficulty = Level.Medium;
                    break;
                case "Hard":
                    Difficulty = Level.Hard;
                    break;
            }
            DrawLevel();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawLevel();
        }

        public bool AddMine(ref int r1, ref int r2)
        {
            if (MineCollection[r1, r2] == false)
            {
                MineCollection[r1, r2] = true;
                return true;
            }
            return false;
        }
    }
}
