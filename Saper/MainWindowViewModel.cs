using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Saper
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ButtonCommand SetDifficulty { get { return new ButtonCommand(SetDifficultyExcute, CanExcute); } }
        public ICommand StartGame { get { return new ButtonCommand(StartGameExecute, CanExcute);} }
        private void SetDifficultyExcute()
        {
            
        }

        private void StartGameExecute()
        {
            
        }

        private bool CanExcute()
        {
            //Some logic
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => {};
    }
}
