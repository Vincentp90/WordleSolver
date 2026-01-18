using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WordleSolver
{
    public enum KeyState
    {
        Unknown,
        Absent,
        Present,
        Correct
    }

    public class KeyboardKey : INotifyPropertyChanged
    {
        public string Letter { get; }

        private KeyState _state = KeyState.Unknown;
        public KeyState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public KeyboardKey(string letter) => Letter = letter;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
