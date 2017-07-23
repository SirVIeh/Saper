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
        #region Public Members

        public enum Level { Easy, Medium, Hard};
        public static Level Difficulty = Level.Easy;
        public List<Button> Buttons = new List<Button>();
        public bool[,] MineCollection;
        public int ColumnCount = 0;
        public int MinesCount = 0;
        public Random randomMinePlace = new Random();

        #endregion
        
        #region Contructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Draw Level

        public void DrawLevel()
        {
            #region Clearing Level
            MainGrid.Children.Clear();
            if (MainGrid.RowDefinitions.Count > 0 && MainGrid.ColumnDefinitions.Count > 0)
            {
                MainGrid.RowDefinitions.RemoveRange(0, MainGrid.RowDefinitions.Count - 1);
                MainGrid.ColumnDefinitions.RemoveRange(0, MainGrid.ColumnDefinitions.Count - 1);
            }
            ColumnCount = 0;
            #endregion

            #region Setting Difficulty and Placing Mines
            switch (Difficulty)
            {
                case Level.Easy:
                    ColumnCount = 10;
                    PlaceMines();
                    break;
                case Level.Medium:
                    ColumnCount = 20;
                    PlaceMines();
                    break;
                case Level.Hard:
                    ColumnCount = 30;
                    PlaceMines();
                    break;
            }
            #endregion

            #region Adding Buttons To Level
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
            #endregion
        }

        #endregion

        #region Mouse Events

        void button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Button b = (Button)sender;
            if (e.ChangedButton == MouseButton.Right && b.Content == null && b.Background != Brushes.White)
            {
                b.Content = "Flaged";
                MinesCount--;
                MinesToFindTextBlock.Text = "Mines to find = " + MinesCount;
            }
            else if (e.ChangedButton == MouseButton.Right && b.Content == "Flaged")
            {
                b.Content = null;
                MinesCount++;
                MinesToFindTextBlock.Text = "Mines to find = " + MinesCount;
            }
            if (MinesCount == 0)
                Win();
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            int x = Convert.ToInt32(b.Name.Substring(2, 1));
            int y = Convert.ToInt32(b.Name.Substring(3, 1));
            var hasMine =
                MineCollection[x, y];
            if (b.Content != "Flaged")
            {
                if (hasMine)
                {
                    MessageBox.Show("Przegrałeś. Spróbuj ponownie.");
                    DrawLevel();
                }
                else
                {
                    //policzyć ile min wokół tego jest i wstawić
                    int minesAround = FindMinesAround(x, y);
                    b.Background = Brushes.White;
                    b.Foreground = Brushes.Black;
                    if (minesAround > 0)
                        b.Content = minesAround;
                }
            }
            //MessageBox.Show(b.Name + "Have i mine ? = " + hasMine);
        }

        private void Difficulty_onClick(object sender, RoutedEventArgs e)
        {
            var difChoice = (MenuItem)sender;
            string difName = difChoice.Header.ToString().Substring(1, difChoice.Header.ToString().Count() - 1);

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

        #endregion

        #region Private Methods

        private void Win()
        {
            MessageBox.Show("You win.\nCongratulations.");
        }

        private void PlaceMines()
        {
            MinesCount = ColumnCount*2;
            MinesToFindTextBlock.Text = "Mines to find = " + MinesCount;
            MineCollection = new bool[ColumnCount, ColumnCount];
            for (int i = 0; i < MinesCount; i++)
            {
                int r1 = randomMinePlace.Next(DateTime.Now.Millisecond % ColumnCount);
                int r2 = randomMinePlace.Next(DateTime.Now.Millisecond % ColumnCount);
                while (!AddMine(r1, r2))
                {
                    r1 = randomMinePlace.Next((DateTime.Now.Millisecond + i) % ColumnCount);
                    r2 = randomMinePlace.Next((DateTime.Now.Millisecond + i) % ColumnCount);
                }
            }
        }

        private int FindMinesAround(int x, int y)
        {
            int minesAround = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if ((i >= 0 && i < ColumnCount) && (j < ColumnCount && j >= 0))
                    {
                        if (MineCollection[i, j])
                            minesAround++;
                    }
                }
            }
            return minesAround;
        }

        private bool AddMine(int r1, int r2)
        {
            if (MineCollection[r1, r2] == false)
            {
                MineCollection[r1, r2] = true;
                return true;
            }
            return false;
        }

        #endregion

        #region On Loaded

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawLevel();
        }

        #endregion
    }
}
