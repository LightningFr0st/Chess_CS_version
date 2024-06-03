using DisplayObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Knight : ChessObject
    {
        public Knight(int x1, int y1, int x2, int y2, string color, bool isMine) : base(x1, y1, x2, y2, color, "n", isMine)
        {
            score = 3;
        }

        public override void updateMoves(byte[,] capture_board)
        {
            Moves.Clear();
            AttackedCells.Clear();
            (int, int)[] possibleMoves = [(-2, -1), (-2, +1), (+2, -1), (+2, +1), (-1, -2), (+1, -2), (+1, +2), (-1, +2)];
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                int cpi = this.i + possibleMoves[i].Item1;
                int cpj = this.j + possibleMoves[i].Item2;
                if (cpi >= 0 && cpi < 8 && cpj >= 0 && cpj < 8)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] != myIND)
                        Moves.Add((cpi, cpj));
                }
            }
        }
    }
}
