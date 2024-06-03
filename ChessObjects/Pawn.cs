using DisplayObjects;   
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Chess
{
    internal class Pawn : ChessObject
    {
        private bool isFirtMove = true;
        private int moveDir = 1;
        public Pawn(int x1, int y1, int x2, int y2, string color, bool isMine) : base(x1, y1, x2, y2, color, "p", isMine)
        {
            score = 1;
            if (isMine)
            {
                moveDir = -1;
            }
        }

        public void PawnMoved()
        {
            isFirtMove = false;
        }

        public override void updateMoves(byte[,] capture_board)
        {
            Moves.Clear();
            AttackedCells.Clear();
            int cpi, cpj;
            for (int i = 1; i <= (isFirtMove ? 2 : 1); i++)
            {
                cpi = this.i + moveDir * i;
                cpj = this.j;
                if (cpi > 0 && capture_board[cpi, this.j] == 0)
                {
                    Moves.Add((cpi, cpj));
                }
                else
                {
                    break;
                }
            }
            cpi = this.i + moveDir;
            cpj = this.j + 1;
            if (cpi >= 0 && cpj < 8)
            {
                AttackedCells.Add((cpi, cpj));
                if (capture_board[cpi,cpj] == foeIND)
                    Moves.Add((cpi, cpj));
            }
            cpj -= 2;
            if (cpi >= 0 && cpj >= 0)
            {
                AttackedCells.Add((cpi, cpj));
                if (capture_board[cpi, cpj] == foeIND)
                    Moves.Add((cpi, cpj));
            }
        }

        public Queen Promote()
        {
            return new Queen(frameX1, frameY1, frameX2, frameY2, color, isMine);
        }
    }
}
