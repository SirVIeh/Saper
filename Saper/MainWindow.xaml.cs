using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public Random RandomMinePlace = new Random();
        public List<string> FlaggedMines = new List<string>();
        public List<Button> uncoveredButtons = new List<Button>();
        public string backgroundBrush = "Black";

        #endregion

        #region Contructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Draw Level

        /// <summary>
        /// Adding all elements to game level
        /// </summary>
        public void DrawLevel()
        {
            #region Clearing Level

            MainGrid.Children.Clear();
            if (MainGrid.RowDefinitions.Count > 0 && MainGrid.ColumnDefinitions.Count > 0)
            {
                MainGrid.RowDefinitions.RemoveRange(0, MainGrid.RowDefinitions.Count - 1);
                MainGrid.ColumnDefinitions.RemoveRange(0, MainGrid.ColumnDefinitions.Count - 1);
            }
            Buttons.Clear();
            FlaggedMines.Clear();
            uncoveredButtons.Clear();
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
                    button.Name = string.Format("B_{0}_{1}", i, j);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    button.Click += button_Click;
                    button.MouseUp += button_MouseUp;
                    if(backgroundBrush == "Red")
                        button.Background = Brushes.Red;
                    if(backgroundBrush == "Green")
                        button.Background = Brushes.Green;
                    if(backgroundBrush == "Blue")
                        button.Background = Brushes.Blue;
                    if(backgroundBrush == "Orange")
                        button.Background = Brushes.OrangeRed;
                    if(backgroundBrush == "Purple")
                        button.Background = Brushes.Purple;
                    if(backgroundBrush == "Black")
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

        /// <summary>
        /// Event to set up flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Button b = (Button)sender;
            var xy = b.Name.Substring(2, b.Name.Length - 2).Split('_').ToArray();
            if (e.ChangedButton == MouseButton.Right && b.Content == null && b.Background != Brushes.White && MinesCount > 0)
            {
                b.Content = "Flagged";
                FlaggedMines.Add(string.Format("{0}_{1}", xy[0], xy[1]));
                MinesCount--;
                MinesToFindTextBlock.Text = "Mines to find = " + MinesCount;
            }
            else if (e.ChangedButton == MouseButton.Right && b.Content == "Flagged")
            {
                b.Content = null;
                FlaggedMines.Remove(string.Format("{0}_{1}", xy[0], xy[1]));
                MinesCount++;
                MinesToFindTextBlock.Text = "Mines to find = " + MinesCount;
            }
            if (MinesCount == 0)
                CheckWinCondition();
        }

        /// <summary>
        /// event to open a field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            var xy = b.Name.Substring(2, b.Name.Length - 2).Split('_').ToArray();
            int x = Convert.ToInt32(xy[0]);
            int y = Convert.ToInt32(xy[1]);
            var hasMine =
                MineCollection[x, y];
            if (b.Content == null)
            {
                if (hasMine)
                {
                    MessageBox.Show("Przegrałeś. Spróbuj ponownie.");
                    DrawLevel();
                }
                else
                {
                    int minesAround = FindMinesAround(x, y);
                    if (minesAround == 0)
                    {
                        uncoveredButtons.Add(b);
                        ShowAllNearEmptyFields(x, y);
                    }
                    b.Background = Brushes.White;
                    b.Foreground = Brushes.Black;
                    if (minesAround > 0)
                        b.Content = minesAround;
                }
            }
            else if (b.Content != "Flagged")
            {
                var minesCount = Convert.ToInt32(b.Content);
                if (CheckIfCorrectlyFlagged(x, y))
                {
                    int minesAround = FindMinesAround(x, y);
                    ShowAllNearEmptyFields(x, y);
                    b.Background = Brushes.White;
                    b.Foreground = Brushes.Black;
                    if (minesAround > 0)
                        b.Content = minesAround;
                }
            }
        }

        /// <summary>
        /// Checking if all that are flagged are correctly flagged
        /// </summary>
        /// <param name="x">x field coordinate</param>
        /// <param name="y">y field coordinate</param>
        /// <returns></returns>
        private bool CheckIfCorrectlyFlagged(int x, int y)
        {
            int minesAround = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if ((i >= 0 && i < ColumnCount) && (j < ColumnCount && j >= 0))
                    {
                        int index = Buttons.FindIndex(0, Buttons.Count,
                                    button => button.Name == string.Format("B_{0}_{1}", i, j));

                        if (MineCollection[i, j] && Buttons[index].Content == "Flagged")
                        {
                            minesAround++;
                        }
                    }
                }
            }
            int idx = Buttons.FindIndex(0, Buttons.Count,
                                    button => button.Name == string.Format("B_{0}_{1}", x, y));
            if (minesAround == Convert.ToInt32(Buttons[idx].Content))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changing difficulty level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Starting new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGame_onClick(object sender, RoutedEventArgs e)
        {
            DrawLevel();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Opening all empty fields around clicked one if possible
        /// </summary>
        /// <param name="x">x field coordinate</param>
        /// <param name="y">y field coordinate</param>
        private void ShowAllNearEmptyFields(int x, int y)
        {
            List<Button> ToUncover = new List<Button>();
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if ((i >= 0 && i < ColumnCount) && (j < ColumnCount && j >= 0))
                    {
                        int index = Buttons.FindIndex(0, Buttons.Count,
                                    button => button.Name == string.Format("B_{0}_{1}", i, j));
                        int idx = Buttons.FindIndex(0, Buttons.Count,
                            button => button.Name == string.Format("B_{0}_{1}", x, y));

                        if (Buttons[index].Content != "Flagged")
                        {
                            if (Buttons[idx].Name != Buttons[index].Name)
                                ToUncover.Add(Buttons[index]);
                            Buttons[index].Background = Brushes.White;
                            Buttons[index].Foreground = Brushes.Black;
                            if (FindMinesAround(i, j) > 0)
                                Buttons[index].Content = FindMinesAround(i, j);
                        }
                    }
                }
            }
            UncoverRestButtons(ToUncover);
        }

        /// <summary>
        /// Uncovering more empty buttons if next to each other
        /// </summary>
        /// <param name="ToUncover"></param>
        public void UncoverRestButtons(List<Button> ToUncover)
        {
            foreach (Button ub in ToUncover)
            {
                if (ub.Content == null && uncoveredButtons.FindIndex(0, uncoveredButtons.Count, button => button.Name == ub.Name) < 0)
                {
                    button_Click(ub, null);
                }
            }
        }

        /// <summary>
        /// Checking if the game is alreade won
        /// </summary>
        private void CheckWinCondition()
        {
            bool wrongFlag = false;
            foreach (string flagged in FlaggedMines)
            {
                var xy = flagged.Split('_').ToArray();
                int x = Convert.ToInt32(xy[0]);
                int y = Convert.ToInt32(xy[1]);
                if (MineCollection[x, y] != true)
                {
                    wrongFlag = true;
                    break;
                }
            }
            if(!wrongFlag)
                MessageBox.Show("You win.\nCongratulations.");
        }

        /// <summary>
        /// Placing mines at random places
        /// </summary>
        private void PlaceMines()
        {
            if(Difficulty == Level.Easy)
                MinesCount = ColumnCount * 2;
            else if(Difficulty == Level.Medium)
                MinesCount = ColumnCount * 2 + 10;
            else
            {
                MinesCount = ColumnCount * 3;
            }
            
            MinesToFindTextBlock.Text = "Mines to find = " + MinesCount;
            MineCollection = new bool[ColumnCount, ColumnCount];
            for (int i = 0; i < MinesCount; i++)
            {
                int r1 = RandomMinePlace.Next() % ColumnCount;
                Thread.Sleep(12);
                int r2 = RandomMinePlace.Next() % ColumnCount;
                Thread.Sleep(12);
                while (!AddMine(r1, r2))
                {
                    r1 = RandomMinePlace.Next() % ColumnCount;
                    Thread.Sleep(12);
                    r2 = RandomMinePlace.Next() % ColumnCount;
                    Thread.Sleep(12);
                }
            }
        }

        /// <summary>
        /// Chacking how many mines are around a field to add it's number on the field
        /// </summary>
        /// <param name="x">x field coordinate</param>
        /// <param name="y">y field coordinate</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adding mines to specified field in parameters
        /// </summary>
        /// <param name="r1">filed coordinate</param>
        /// <param name="r2">filed coordinate</param>
        /// <returns></returns>
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

        /// <summary>
        /// Starting game when window loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawLevel();
        }

        #endregion

        /// <summary>
        /// Changing fileds color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Color_Click(object sender, RoutedEventArgs e)
        {
            Button cb = (Button)sender;
            switch (cb.Content.ToString())
            {
                case "Red":
                    foreach (Button button in Buttons)
                    {
                        if(button.Background != Brushes.White)
                            button.Background = Brushes.Red;
                    }
                    backgroundBrush = "Red";
                    break;
                case "Green":
                    foreach (Button button in Buttons)
                    {
                        if (button.Background != Brushes.White)
                            button.Background = Brushes.Green;
                    }
                    backgroundBrush = "Green";
                    break;
                case "Blue":
                    foreach (Button button in Buttons)
                    {
                        if (button.Background != Brushes.White)
                            button.Background = Brushes.Blue;
                    }
                    backgroundBrush = "Blue";
                    break;
                case "Orange":
                    foreach (Button button in Buttons)
                    {
                        if (button.Background != Brushes.White)
                            button.Background = Brushes.OrangeRed;
                    }
                    backgroundBrush = "Orange";
                    break;
                case "Purple":
                    foreach (Button button in Buttons)
                    {
                        if (button.Background != Brushes.White)
                            button.Background = Brushes.Purple;
                    }
                    backgroundBrush = "Purple";
                    break;
                case "Black":
                    foreach (Button button in Buttons)
                    {
                        if (button.Background != Brushes.White)
                            button.Background = Brushes.Black;
                    }
                    backgroundBrush = "Black";
                    break;
            }
        }
    }
}
