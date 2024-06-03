using DisplayObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Rook : ChessObject
    {
        public Rook(int x1, int y1, int x2, int y2, string color, bool isMine) : base(x1, y1, x2, y2, color, "r", isMine)
        {
            score = 5;
        }

        public override void updateMoves(byte[,] capture_board)
        {
            Moves.Clear();
            AttackedCells.Clear();
            AddRow(capture_board);
            AddCol(capture_board);
        }
    }
}
