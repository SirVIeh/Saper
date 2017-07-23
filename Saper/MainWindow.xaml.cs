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
            int columnCount = 0;
            switch (Difficulty)
            {
                case Level.Easy:
                    columnCount = 10;
                    break;
                case Level.Medium:
                    columnCount = 20;
                    break;
                case Level.Hard:
                    columnCount = 30;
                    break;
            }

            for (int i = 0; i < columnCount; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(column);
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                MainGrid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    Button button = new Button();
                    button.Name = string.Format("B_{0}{1}", i, j);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    button.Click += button_Click;
                    Buttons.Add(button);
                    MainGrid.Children.Add(button);
                }
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            MessageBox.Show(b.Name);
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
    }
}
