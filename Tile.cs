using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WordleSolver
{
    public enum TileState
    {
        Empty,
        Correct,   // Green
        Present,   // Yellow
        Absent     // Gray
    }

    public class Tile : INotifyPropertyChanged
    {
        private string _letter;
        public string Letter
        {
            get => _letter;
            set { _letter = value; OnPropertyChanged(nameof(Letter)); }
        }

        private TileState _state;
        public TileState State
        {
            get => _state;
            set { _state = value; OnPropertyChanged(nameof(State)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
