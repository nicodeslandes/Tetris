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

        public async Task Start()
        {
            var rand = new Random();
            while (true)
            {
                var changes = new List<CellChange>();
                for (int x = 0; x < Cells.GetLength(0); x++)
                {
                    for (int y = 0; y < Cells.GetLength(1); y++)
                    {
                        Cells[x, y] = Color.FromRgb(
                              (byte)rand.Next(256),
                              (byte)rand.Next(256),
                              (byte)rand.Next(256)
                          );
                        changes.Add(new CellChange(x, y, Cells[x,y]));
                    }
                }

                _changesPump.OnNext(new CellChanges(changes.ToArray()));
                await Task.Delay(10);
            }
        }

        public IObservable<CellChanges> CellChanges { get; }
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
