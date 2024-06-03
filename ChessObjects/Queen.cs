using DisplayObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Queen : ChessObject
    {
        public Queen(int x1, int y1, int x2, int y2, string color, bool isMine) : base(x1, y1, x2, y2, color, "q", isMine)
        {
            score = 9;
        }

        public override void updateMoves(byte[,] capture_board)
        {
            Moves.Clear();
            AttackedCells.Clear();
            AddCol(capture_board);
            AddRow(capture_board);
            AddMainDiag(capture_board);
            AddSideDiag(capture_board);
        }

    }
}
