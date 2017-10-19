using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Tetris.Models
{
    class GameBoard
    {
        public const int BoardSizeX = 10;
        public const int BoardSizeY = 20;

        private readonly Subject<CellChanges> _changesPump = new Subject<CellChanges>();

        public GameBoard()
        {
            CellChanges = _changesPump.AsObservable();
            Cells[0, 0] = Colors.Red;
            Cells[1, 0] = Colors.Cyan;
            Cells[2, 0] = Colors.Yellow;
            Cells[3, 0] = Colors.Red;
        }

        public Color?[,] Cells { get; } = new Color?[10, 20];

        class Cell
        {
            public Color? Color { get; set; }

            enum State
            {
                Empty,
                MovingPiece,
                Filled
            }
        }

        public void HandleKeyPress(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    ShiftCurrentPiece((-1, 0));
                    break;
                case Key.Right:
                    ShiftCurrentPiece((1, 0));
                    break;
                case Key.Down:
                    ShiftCurrentPiece((0, -1));
                    break;
            }
        }

        class Movement
        {
            public Movement((int dx, int dy) m)
            {
                Dx = m.dx;
                Dy = m.dy;
            }

            public int Dx { get; }
            public int Dy { get; }

            public static implicit operator Movement((int dx, int dy) m)
            {
                return new Movement(m);
            }

        }

        Piece currentPiece;
        (int x, int y) position = (x: 5, y: 15);

        public async Task Start()
        {
            var rand = new Random();
            while (true)
            {
                currentPiece = GetNewPiece();
                position = (x: 5, y: 18);

                do
                {
                    ShiftCurrentPiece((dx: 0, dy: -1));
                    await Task.Delay(500);
                } while (position.y > 0);
            }
        }

        void ShiftCurrentPiece(Movement m)
        {
            var changes = new List<CellChange>();

            void SetCell(int x, int y, Color? color)
            {
                if (x < 0 || x >= BoardSizeX || y < 0 || y >= BoardSizeY)
                    return;

                if (Cells[x, y] != color)
                {
                    changes.Add(new CellChange(x, y, color));
                    Cells[x, y] = color;
                }
            }

            // Clear all cells currently covered by the current piece
            // TODO: We could avoid clearing cells that we still
            // be covered after the pieve has moved
            for (int i = 0; i < 16; i++)
            {
                if (currentPiece.Cells[i])
                {
                    var (px, py) = (i % 4, i / 4);
                    SetCell(position.x + px, position.y + py, null);
                }
            }

            // Update the piece's position
            position.x += m.Dx;
            position.y += m.Dy;

            // Fill all the cells for the new position
            for (int i = 0; i < 16; i++)
            {
                var (px, py) = (i % 4, i / 4);
                SetCell(position.x + px, position.y + py,
                    currentPiece.Cells[i] ? currentPiece.Color : (Color?)null);
            }

            _changesPump.OnNext(new CellChanges(changes.ToArray()));
        }

        private Piece GetNewPiece()
        {
            return new Piece(new[]
            {
                true, false, false, false,
                true, false, false, false,
                true, true, false, false,
                false, false, false, false
            }, Colors.DarkBlue);
        }

        public IObservable<CellChanges> CellChanges { get; }
    }

    internal class Piece
    {
        public Piece(bool[] cells, Color color)
        {
            Cells = cells;
            Color = color;
        }
        public bool[] Cells { get; }
        public Color Color { get; }
    }

    internal class CellChanges
    {
        public CellChanges(CellChange[] changes)
        {
            Changes = changes;
        }

        public CellChange[] Changes { get; }
    }

    internal class CellChange
    {
        public CellChange(int x, int y, Color? color)
        {
            this.CellX = x;
            this.CellY = y;
            Color = color;
        }

        public int CellX { get; set; }
        public int CellY { get; set; }

        public Color? Color { get; }
    }
}
