using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using WordleSolver.Views;

namespace WordleSolver.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView = new GameView();
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowGameCommand =>
            new RelayCommand(_ => CurrentView = new GameView());

        public ICommand ShowSolveOtherCommand =>
            new RelayCommand(_ => CurrentView = new SolveOtherView());

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
