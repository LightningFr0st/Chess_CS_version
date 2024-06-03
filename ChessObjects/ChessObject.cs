using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using DisplayObjects;
using Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Chess
{
    abstract class ChessObject : RectangleObject
    {
        protected int score;

        protected int foeIND = 2;
        protected int myIND = 1;


        protected string type;
        protected string color;
        public bool isMine;
        private static string txtrDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @".\assets\"));
        public List<(int, int)> Moves = new List<(int, int)>();
        public List<(int, int)> AttackedCells = new List<(int, int)>();



        public ChessObject(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, string color, 
            string type, bool side) :
        base(topLeftX, topLeftY, bottomRightX, bottomRightY, getTexture(color, type))
        {
            this.type = type;
            this.color = color;
            clickHandler = Select;
            isMine = side;
            if (!isMine)
            {
                myIND = 2;
                foeIND = 1;
            }
            RenewPos();
        }

        private static Bitmap getTexture(string color, string type)
        {
            Bitmap img = new Bitmap(new Bitmap(txtrDir + type + color + ".png"), new Size(ObjectGenerator.SQUARE_SIZE, ObjectGenerator.SQUARE_SIZE));
            return img;
        }

        public void RenewPos()
        {
            i = anchorY / height;
            j = anchorX / width;
        }

        public void Select()
        {
            //Console.WriteLine("{0}{1}: {2},{3}", color, type, anchorX, anchorY);
        }

        public virtual DisplayObject[] getMoves(ref byte[,] capture_board)
        {
            //updateMoves(ref capture_board);
            List<DisplayObject> movesObj = new List<DisplayObject>();
            for (int i = 0; i < Moves.Count; i++)
            {
                CircleObject co = new CircleObject(Moves[i].Item2 * height + height / 2, Moves[i].Item1 * width + width / 2, width / 6, Color.FromArgb(202, 203, 179));

                co.clickHandler = () => { Console.WriteLine("Moved to: {0},{1}", co.anchorX, co.anchorY); };
                movesObj.Add(co);
            }
            return movesObj.ToArray();
        }

        public abstract void updateMoves(byte[,] capture_board);

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            //Brush borderBursh = GetStrokeBrush();
            //Rectangle rect = new Rectangle(frameX1, frameY1, frameX2 - frameX1, frameY2 - frameY1);
            //g.DrawRectangle(new Pen(borderBursh, 2), rect);
        }



        //utility

        public void AddCol(byte[,] capture_board)
        {
            int cpi = i - 1;
            int cpj = j;

            while (cpi >= 0)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                Moves.Add((cpi, cpj));
                AttackedCells.Add((cpi, cpj));
                cpi--;
            }
            cpi = i + 1;
            while (cpi < 8)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                AttackedCells.Add((cpi, cpj));
                Moves.Add((cpi, cpj));
                cpi++;
            }
        }

        public void AddRow(byte[,] capture_board)
        {
            int cpi = i;
            int cpj = j - 1;
            while (cpj >= 0)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                Moves.Add((cpi, cpj));
                AttackedCells.Add((cpi, cpj));
                cpj--;
            }
            cpj = j + 1;
            while (cpj < 8)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                Moves.Add((cpi, cpj));
                AttackedCells.Add((cpi, cpj));
                cpj++;
            }
        }

        public void AddMainDiag(byte[,] capture_board)
        {
            int cpi = i - 1;
            int cpj = j - 1;
            while (cpj >= 0 && cpi >= 0)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                AttackedCells.Add((cpi, cpj));
                Moves.Add((cpi, cpj));
                cpj--;
                cpi--;
            }
            cpi = i + 1;
            cpj = j + 1;
            while (cpj < 8 && cpi < 8)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                AttackedCells.Add((cpi, cpj));
                Moves.Add((cpi, cpj));
                cpj++;
                cpi++;
            }
        }

        public void AddSideDiag(byte[,] capture_board)
        {
            int cpi = i + 1;
            int cpj = j - 1;
            while (cpj >= 0 && cpi < 8)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                AttackedCells.Add((cpi, cpj));
                Moves.Add((cpi, cpj));
                cpj--;
                cpi++;
            }
            cpi = i - 1;
            cpj = j + 1;
            while (cpj < 8 && cpi >= 0)
            {
                if (capture_board[cpi, cpj] != 0)
                {
                    AttackedCells.Add((cpi, cpj));
                    if (capture_board[cpi, cpj] == foeIND)
                    {
                        Moves.Add((cpi, cpj));
                    }
                    break;
                }
                AttackedCells.Add((cpi, cpj));
                Moves.Add((cpi, cpj));
                cpj++;
                cpi--;
            }
        }

    }
}
