using DisplayObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Bishop : ChessObject
    {
        public Bishop(int x1, int y1, int x2, int y2, string color, bool isMine) : base(x1, y1, x2, y2, color, "b", isMine)
        {
            score = 3;
        }

        public override void updateMoves(byte[,] capture_board)
        {
            Moves.Clear();
            AttackedCells.Clear();
            AddMainDiag(capture_board);
            AddSideDiag(capture_board);

        }
    }
}
