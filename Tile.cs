using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

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

    public class ClickableTile : Tile
    {
        public ICommand CycleStateCommand { get; }

        public ClickableTile()
        {
            CycleStateCommand = new RelayCommand(_ => CycleState());
        }

        private void CycleState()
        {
            State = State switch
            {
                TileState.Absent => TileState.Present,
                TileState.Present => TileState.Correct,
                TileState.Correct => TileState.Absent,
                _ => TileState.Absent
            };
        }
    }
}
