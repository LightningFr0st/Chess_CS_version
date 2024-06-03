using Chess;
using DisplayObjects;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Numerics;
using System.Text;

namespace Utilities
{
    internal class ObjectGenerator
    {
        private static Random rand = new Random();

        private static string your_color = "w";
        private static string your_color_full = "White";
        private static string enemy_color = "b";
        private static string enemy_color_full = "Black";

        public static int SQUARE_SIZE;

        private static int actualW = 0, actualH = 0;

        private static int border;

        private static string txtrDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @".\assets\"));

        delegate ChessObject GetChessPiece(int x1, int y1, int x2, int y2, string typem, bool isMine);
        private static GetChessPiece[] genDelegates = [getRook, getKnight, getBishop, getQueen, getKing, getPawn];

        public static void setPlayersColors(char temp)
        {
            if (temp == 'b')
            {
                your_color = "b";
                your_color_full = "Black";

                enemy_color = "w";
                enemy_color_full = "White";
            }
        }

        public static ChessBoard BOARD;

        public static ChessBoard GenChessBoard(int leftTopX, int leftTopY, int bottomRightX, int bottomRightY, int thickness)
        {

            BOARD = new ChessBoard(leftTopX, leftTopY, bottomRightX, bottomRightY, thickness);
            border = thickness;
            BOARD.fillColor = Color.FromArgb(255, 255, 218);
            BOARD.borderColor = Color.FromArgb(102, 76, 5);

            InitializeGenerators(bottomRightX - leftTopX, bottomRightY - leftTopY, thickness);

            RectangleObject[,] board = genBoard();
            TextObject[] letters = genLetters();
            List<ChessObject> player;
            List<ChessObject> enemy;

            (player, enemy) = genPieces();

            BOARD.InitArrays(board, letters, enemy, player);

            return BOARD;

        }


        public static GameField GenStartField(int leftTopX, int leftTopY, int bottomRightX, int bottomRightY, int thickness)
        {
            GameField StarField = new GameField(leftTopX, leftTopY, bottomRightX, bottomRightY, thickness);

            RectangleObject[] objects = new RectangleObject[17];

            RectangleObject obj;
            int x, y = 0;

            bool isBlack = false;

            for (int i = 0; i < 4; i++)
            {
                x = 0;
                if (i % 2 == 0)
                {
                    isBlack = false;
                }
                else
                {
                    isBlack = true;
                }
                for (int j = 0; j < 4; j++)
                {
                    // acquire current generation 
                    obj = getTile(x, y, x + 100, y + 100);
                    x += 100;
                    if (isBlack)
                    {
                        obj.fillColor = Color.FromArgb(115, 149, 82);
                        isBlack = false;
                    }
                    else
                    {
                        obj.fillColor = Color.FromArgb(235, 236, 208);
                        isBlack = true;
                    }
                    objects[i * 4 + j] = obj;
                }
                y += 100;
            }

            int width = 512 / 4;
            int height = 1274 / 4;
            Bitmap img = new Bitmap(new Bitmap(txtrDir + "TITLE.png"), new Size(width, height));
            RectangleObject TITLE = new RectangleObject(220, 30, 220 + width, 30 + height, img);
            TITLE.strokeThickness = 0;

            objects[16] = TITLE;

            for (int i = 0; i < 17; i++)
            {
                StarField.AddObject(objects[i]);
            }

            return StarField;
        }

        public static GameField GenEndField(int leftTopX, int leftTopY, int bottomRightX, int bottomRightY, int thickness, bool isWin)
        {
            GameField EndField = new GameField(leftTopX, leftTopY, bottomRightX, bottomRightY, thickness);
            EndField.fillColor = Color.FromArgb(235, 236, 208);
            EndField.borderColor = Color.Black;
            EndField.strokeThickness = thickness;
            string text;
            if (isWin)
            {
                text = your_color_full + " won";
            }
            else
            {
                text = enemy_color_full + " won";
            }
            TextObject result = new TextObject(text, 100, 50, 300, 130, null, 20, null);
            EndField.AddObject(result);
            return EndField;
        }

        public static TextObject[] genLetters()
        {
            TextObject[] letters = new TextObject[16];
            TextObject obj;
            int num = 8;
            char let = 'a';
            int x = -border;
            int y = 0;
            for (int i = 0; i < 8; i++)
            {
                obj = getText(x, y, 0, y + SQUARE_SIZE, num.ToString());
                letters[i] = obj;
                y += SQUARE_SIZE;
                num--;
            }
            x = 0; num = 0;
            for (int i = 8; i < 16; i++)
            {
                obj = getText(x, y, x + SQUARE_SIZE, y + border, let.ToString());
                //obj = getText(x, y, x + SQUARE_SIZE, y + border, num.ToString());
                letters[i] = obj;
                x += SQUARE_SIZE;
                //num++;
                let++;
            }
            return letters;
        }

        public static RectangleObject[,] genBoard()
        {
            RectangleObject[,] board = new RectangleObject[8, 8];
            RectangleObject obj;
            int x, y = 0;

            bool isBlack = false;

            for (int i = 0; i < 8; i++)
            {
                x = 0;
                if (i % 2 == 0)
                {
                    isBlack = false;
                }
                else
                {
                    isBlack = true;
                }
                for (int j = 0; j < 8; j++)
                {
                    // acquire current generation 
                    obj = getTile(x, y, x + SQUARE_SIZE, y + SQUARE_SIZE);
                    x += SQUARE_SIZE;
                    if (isBlack)
                    {
                        obj.fillColor = Color.FromArgb(115, 149, 82);
                        isBlack = false;
                    }
                    else
                    {
                        obj.fillColor = Color.FromArgb(235, 236, 208);
                        isBlack = true;
                    }
                    board[i, j] = obj;
                }
                y += SQUARE_SIZE;
            }
            return board;
        }

        private static (List<ChessObject>, List<ChessObject>) genPieces()
        {
            List<ChessObject> player = new List<ChessObject>();
            List<ChessObject> enemy = new List<ChessObject>();
            ChessObject obj;
            int x = 0, y = 0;
            /*
                rook = 0
                knight = 1
                bishop = 2
                queen = 3
                king  = 4
                pawn = 5
            */

            //generating pieces
            int[] chessmen = [0, 1, 2, 3, 4, 2, 1, 0];

            //testing

            //obj = genDelegates[chessmen[0]](0 * SQUARE_SIZE, 0 * SQUARE_SIZE, 1 * SQUARE_SIZE, 1 * SQUARE_SIZE, "w");
            //player.Add(obj);
            //obj = genDelegates[chessmen[2]](5 * SQUARE_SIZE, 6 * SQUARE_SIZE, 6 * SQUARE_SIZE, 7 * SQUARE_SIZE, "w");
            //player.Add(obj);
            //obj = genDelegates[chessmen[3]](1 * SQUARE_SIZE, 2 * SQUARE_SIZE, 2 * SQUARE_SIZE, 3 * SQUARE_SIZE, "w");
            //player.Add(obj);
            //obj = genDelegates[chessmen[1]](3 * SQUARE_SIZE, 3 * SQUARE_SIZE, 4 * SQUARE_SIZE, 4 * SQUARE_SIZE, "w");
            //player.Add(obj);
            //obj = genDelegates[5](0 * SQUARE_SIZE, 6 * SQUARE_SIZE, 1 * SQUARE_SIZE, 7 * SQUARE_SIZE, "w");
            //player.Add(obj);

            for (int i = 0; i < 8; i++)
            {
                //adding your pieces
                obj = genDelegates[chessmen[i]](x, 7 * SQUARE_SIZE, x + SQUARE_SIZE, 8 * SQUARE_SIZE, your_color, true);
                player.Add(obj);
                obj = genDelegates[5](x, 6 * SQUARE_SIZE, x + SQUARE_SIZE, 7 * SQUARE_SIZE, your_color, true);
                player.Add(obj);

                //adding enemy pieces
                obj = genDelegates[chessmen[i]](x, 0, x + SQUARE_SIZE, SQUARE_SIZE, enemy_color, false);
                enemy.Add(obj);
                obj = genDelegates[5](x, SQUARE_SIZE, x + SQUARE_SIZE, 2 * SQUARE_SIZE, enemy_color, false);
                enemy.Add(obj);
                //next cell
                x += SQUARE_SIZE;
            }

            return (player, enemy);
        }

        public static void InitializeGenerators(int fieldWidth, int fieldHeight, int borderThick)
        {
            actualW = fieldWidth - 2 * borderThick;
            actualH = fieldHeight - 2 * borderThick;
            SQUARE_SIZE = actualH / 8;

        }

        public static RectangleObject getTile(int x1, int y1, int x2, int y2)
        {
            RectangleObject ro = new RectangleObject(x1, y1, x2, y2);
            return ro;
        }

        public static ChessObject getRook(int x1, int y1, int x2, int y2, string type, bool isMine)
        {
            Rook ro = new Rook(x1, y1, x2, y2, type, isMine);
            return ro;
        }

        public static ChessObject getBishop(int x1, int y1, int x2, int y2, string type, bool isMine)
        {
            Bishop ro = new Bishop(x1, y1, x2, y2, type, isMine);
            return ro;
        }

        public static ChessObject getKing(int x1, int y1, int x2, int y2, string type, bool isMine)
        {
            King ro = new King(x1, y1, x2, y2, type, isMine, BOARD.player_board, BOARD.enemy_board);
            return ro;
        }

        public static ChessObject getKnight(int x1, int y1, int x2, int y2, string type, bool isMine)
        {
            Knight ro = new Knight(x1, y1, x2, y2, type, isMine);
            return ro;
        }

        public static ChessObject getQueen(int x1, int y1, int x2, int y2, string type, bool isMine)
        {
            Queen ro = new Queen(x1, y1, x2, y2, type, isMine);
            return ro;
        }

        public static ChessObject getPawn(int x1, int y1, int x2, int y2, string type, bool isMine)
        {
            Pawn ro = new Pawn(x1, y1, x2, y2, type, isMine);
            return ro;
        }


        public static TextObject getText(int x1, int y1, int x2, int y2, string text)
        {
            TextObject to = new TextObject(text, x1, y1, x2, y2, FontFamily.GenericMonospace, 14, Color.Black);
            return to;
        }

        //public static DisplayObject getChessman(int x1, int y1, int x2, int y2, string type)
        //{
        //    Bitmap img = new Bitmap(new Bitmap(txtrDir + type + ".png"), new Size(SQUARE_SIZE, SQUARE_SIZE));
        //    RectangleObject ro = new RectangleObject(x1, y1, x2, y2, img);
        //    return ro;
        //}
    }
}
