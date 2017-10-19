using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ReactiveUI;
using Tetris.Annotations;
using Tetris.Models;

namespace Tetris.ViewModels
{
    class GameBoardViewModel
    {
        private const int CellSize = 20;
        private const int BoardSizeX = 10;
        private const int BoardSizeY = 20;

        private readonly CellViewModel[] _cells = new CellViewModel[BoardSizeX * BoardSizeY];

        public GameBoardViewModel()
        {
            var model = new GameBoard();
            StartCommand = ReactiveCommand.CreateFromTask(model.Start);
            KeyPressedCommand = ReactiveCommand.Create<string>(k =>
                model.HandleKeyPress((Key) Enum.Parse(typeof(Key), k)));

            Cells = CreateCellViewModels(model);
            model.CellChanges.Subscribe(OnCellModelChanges);
        }

        private void OnCellModelChanges(CellChanges changes)
        {
            foreach (var change in changes.Changes)
            {
                _cells[change.CellX + BoardSizeX * change.CellY].Color = change.Color ?? Colors.DarkGray;
            }
        }

        private ReadOnlyCollection<CellViewModel> CreateCellViewModels(GameBoard model)
        {
            for (int x = 0; x < BoardSizeX; x++)
            for (int y = 0; y < BoardSizeY; y++)
            {
                _cells[x + BoardSizeX * y] =
                    new CellViewModel(
                        x * (CellSize + 1),
                        (BoardSizeY - y) * (CellSize + 1))
                    {
                        Color = model.Cells[x, y] ?? Colors.DarkGray
                    };
            }

            var cells = new ReadOnlyCollection<CellViewModel>(_cells);
            return cells;
        }

        public ReadOnlyCollection<CellViewModel> Cells { get; }

        public ReactiveCommand StartCommand { get; }

        public ReactiveCommand KeyPressedCommand { get; }
    }

    internal class CellViewModel: INotifyPropertyChanged
    {
        private Color _color;

        public CellViewModel(int x, int y)
        {
            X = x;
            Y = y;
            _color = Colors.DarkGray;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public Color Color
        {
            get => _color;
            set
            {
                if (value.Equals(_color)) return;
                _color = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
