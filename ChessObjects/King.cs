using DisplayObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class King : ChessObject
    {
        private bool isFirtMove = true;

        private bool[,] EnemyDeffense;
        private bool[,] MyDefense;


        public King(int x1, int y1, int x2, int y2, string color, bool isMine, bool[,] player, bool[,] enemy) : base(x1, y1, x2, y2, color, "k", isMine)
        {
            if (isMine)
            {
                EnemyDeffense = enemy;
                MyDefense = player;
            }
            else
            {
                EnemyDeffense = player;
                MyDefense = enemy;
            }
            
        }

        public override void updateMoves(byte[,] capture_board)
        {
            Moves.Clear();
            AttackedCells.Clear();
            (int, int)[] possibleMoves = [(-1, -1), (-1, 0), (-1, +1), (0, -1), (0, +1), (+1, -1), (+1, 0), (+1, +1)];
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                int cpi = this.i + possibleMoves[i].Item1;
                int cpj = this.j + possibleMoves[i].Item2;
                
                if (cpi >= 0 && cpi < 8 && cpj >= 0 && cpj < 8)
                {
                    //!EnemyDeffense[cpi, cpj]
                    //Console.WriteLine("{0}{1}:{2},{3} - {4}", color, type, cpi, cpj, EnemyDeffense[cpi, cpj]);
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] != myIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                        
                }
            }
        }
    }
}
