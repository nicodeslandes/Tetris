using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Models
{
    class GameBoard
    {
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

        public async Task Start()
        {
            var rand = new Random();
            while (true)
            {
                var currentPiece = GetNewPiece();
                var position = (x: 5, y: 15);

                do
                {
                    var changes = new List<CellChange>();

                    void SetCell(int x, int y, Color? color)
                    {
                        if (Cells[x, y] != color)
                        {
                            changes.Add(new CellChange(x, y, color));
                            Cells[x, y] = color;
                        }
                    }

                    void ShiftCurrentPieceDown()
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (currentPiece.Cells[i])
                            {
                                SetCell(position.x + i, position.y, null);
                            }
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            var (px, py) = (i % 4, i / 4);
                            SetCell(position.x + px, position.y + py,
                                currentPiece.Cells[i] ? currentPiece.Color : (Color?) null);
                        }

                        position.y--;
                    }

                    ShiftCurrentPieceDown();

                    _changesPump.OnNext(new CellChanges(changes.ToArray()));
                    await Task.Delay(500);
                } while (position.y > 4);
            }
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
