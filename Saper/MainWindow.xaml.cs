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
        public MainWindow()
        {
            InitializeComponent();
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
                    button.Name = string.Format("{0}{1}", i, j);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    MainGrid.Children.Add(button);
                }
            }
        }
    }
}
