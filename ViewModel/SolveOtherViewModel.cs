using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace WordleSolver.ViewModel
{
    public class SolveOtherViewModel : INotifyPropertyChanged
    {
        #region Wordle Properties
        public ObservableCollection<ObservableCollection<ClickableTile>> Board { get; } = new();
        public ObservableCollection<KeyboardKey> KeyboardKeys { get; } = new();

        private string _feedback;
        public string Feedback
        {
            get => _feedback;
            set
            {
                if (_feedback != value)
                {
                    _feedback = value;
                    OnPropertyChanged(nameof(Feedback));
                }
            }
        }

        private string _currentGuess = "";
        private int _wordLength = 5;
        private int _totalGuesses = 6;
        #endregion

        #region Solver Properties
        private Solver _solver;

        private string _autoGuess;
        public string AutoGuess
        {
            get => _autoGuess;
            set
            {
                if (_autoGuess != value)
                {
                    _autoGuess = value;
                    OnPropertyChanged(nameof(AutoGuess));
                }
            }
        }
        public ICommand ConfirmGuessCommand { get; }

        public ICommand RunSolverCommand { get; }
        #endregion


        public SolveOtherViewModel()
        {
            RunSolverCommand = new RelayCommand(
                    _ => RunSolver(),
                    _ => true
                );

            ConfirmGuessCommand = new RelayCommand(
                    _ => ConfirmGuess(),
                    _ => true
                );

            ResetState();
        }

        #region Wordle Methods
        private void InitBoard()
        {
            Board.Clear();
            for (int r = 0; r < _totalGuesses; r++)
            {
                var row = new ObservableCollection<ClickableTile>();
                for (int c = 0; c < _wordLength; c++)
                    row.Add(new ClickableTile());
                Board.Add(row);
            }
        }

        private void InitKeyBoard()
        {
            KeyboardKeys.Clear();
            foreach (var k in new[]
            {
                "q","w","e","r","t","y","u","i","o","p",
                "a","s","d","f","g","h","j","k","l",
                "z","x","c","v","b","n","m",
                "ä","ö","ü","ß","é"
            })
            {
                KeyboardKeys.Add(new KeyboardKey(k));
            }
        }

        private void ResetState()
        {
            InitBoard();
            InitKeyBoard();
            _feedback = "";

            _solver = new Solver(_wordLength);
            _autoGuess = _solver.Iterate();
        }

        private void UpdateKeyboard(string guess)
        {
            //TODO
        }
        #endregion

        #region Solver Methods
        private void RunSolver()
        {
            var row = Board.Last(r => !r.Any(t => t.State == TileState.Empty));
            AutoGuess = _solver.Iterate(row);
        }

        private void ConfirmGuess()
        {
            var row = Board.First(r => r.Any(t => t.State == TileState.Empty));
            for (int i = 0; i < _wordLength; i++)
            {
                row[i].Letter = i < _autoGuess.Length ? _autoGuess[i].ToString() : "";
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
