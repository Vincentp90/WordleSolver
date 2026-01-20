using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace WordleSolver
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Wordle Properties
        private readonly Random _random = new Random();
        public ObservableCollection<ObservableCollection<Tile>> Board { get; } = new();
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
        private string _answer;
        private int _wordLength = 5;
        private int _totalGuesses = 6;
        #endregion

        #region Solver Properties
        private Solver _solver;

        private string _specificWord;
        public string SpecificWord
        {
            get => _specificWord;
            set
            {
                if (_specificWord != value)
                {
                    _specificWord = value;
                    OnPropertyChanged(nameof(SpecificWord));
                }
            }
        }

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
        public ICommand AutoGuessCommand { get; }

        public ICommand ResetSpecificWordCommand { get; }
        #endregion


        public MainViewModel()
        {
            ResetSpecificWordCommand = new RelayCommand(
                    _ => ResetSpecificWord(),
                    _ => true
                );

            AutoGuessCommand = new RelayCommand(
                    _ => DoAutoGuess(),
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
                var row = new ObservableCollection<Tile>();
                for (int c = 0; c < _wordLength; c++)
                    row.Add(new Tile());
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
                "ä","ö","ü","ß","é", "Delete","Enter","Reset"
            })
            {
                KeyboardKeys.Add(new KeyboardKey(k));
            }
        }

        public void KeyPressed(string key)
        {
            if (key == "Enter")
            {
                SubmitGuess();
            }
            else if (key == "Delete")
            {
                if (_currentGuess.Length > 0)
                {
                    _currentGuess = _currentGuess[..^1];
                    UpdateCurrentRow();
                }
            }
            else if(key == "Reset")
            {
                ResetState();
            }
            else if (_currentGuess.Length < _wordLength)
            {
                if (!Board.Any(r => r.First().State == TileState.Empty))
                {
                    Feedback = $"Word not guessed, the word was: {_answer}";
                    return;
                }
                _currentGuess += key.ToUpper();
                UpdateCurrentRow();
            }
        }

        private void ResetState()
        {
            InitBoard();
            InitKeyBoard();
            _currentGuess = "";
            _answer = App.CommonWords[_random.Next(App.CommonWords.Count)].ToUpper();
            _feedback = "";

            _solver = new Solver(_wordLength);
            _autoGuess = _solver.Iterate();
        }

        private void UpdateCurrentRow()
        {
            var row = Board.First(r => r.Any(t => t.State == TileState.Empty));            
            for (int i = 0; i < _wordLength; i++)
            {
                row[i].Letter = i < _currentGuess.Length ? _currentGuess[i].ToString() : "";
            }
        }

        private void SubmitGuess()
        {
            if (_currentGuess.Length != _wordLength) return;

            if (!App.AllWords.Contains(_currentGuess.ToLower()))
            {
                Feedback = "Word does not exist!";
                return;
            }
            else
                Feedback = "";

            var row = Board.First(r => r.Any(t => string.IsNullOrEmpty(t.Letter) || t.State == TileState.Empty));
            EvaluateRow(row);
            UpdateKeyboard(_currentGuess);

            AutoGuess = _solver.Iterate(row);

            _currentGuess = "";
        }

        private void EvaluateRow(ObservableCollection<Tile> row)
        {
            var remaining = new Dictionary<char, int>();

            foreach (char c in _answer)
            {
                if (!remaining.TryAdd(c, 1))
                    remaining[c]++;
            }
            for (int i = 0; i < _wordLength; i++)
            {
                if (_currentGuess[i] == _answer[i])
                {
                    row[i].State = TileState.Correct;
                    remaining[_currentGuess[i]]--;
                }
            }
            for (int i = 0; i < _wordLength; i++)
            {
                if (row[i].State == TileState.Correct)
                    continue;

                char c = _currentGuess[i];

                if (remaining.TryGetValue(c, out int count) && count > 0)
                {
                    row[i].State = TileState.Present;
                    remaining[c]--;
                }
                else
                {
                    row[i].State = TileState.Absent;
                }
            }
        }

        private void UpdateKeyboard(string guess)
        {
            for (int i = 0; i < _wordLength; i++)
            {
                var key = KeyboardKeys.FirstOrDefault(k => k.Letter == guess[i].ToString().ToLower());
                if (key == null) continue;

                if (_answer[i] == guess[i])
                    key.State = KeyState.Correct;
                else if (_answer.Contains(guess[i]) && key.State != KeyState.Correct)
                    key.State = KeyState.Present;
                else if (key.State == KeyState.Unknown)
                    key.State = KeyState.Absent;
            }
        }
        #endregion

        #region Solver Methods
        private void ResetSpecificWord()
        {
            ResetState();
            _answer = SpecificWord.ToUpper();
        }

        private void DoAutoGuess()
        {
            foreach(var letter in _autoGuess)
                KeyPressed(letter.ToString());
            KeyPressed("Enter");
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
