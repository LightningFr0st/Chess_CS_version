using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisplayObjects;

namespace Chess
{
    internal class ChessBoard : GameField
    {
        public bool ENDTURN = false;
        public int STARTI;
        public int STARTJ;
        public int DESTI;
        public int DESTJ;

        private TextObject[] letters = new TextObject[16];

        private List<ChessObject> enemy = new List<ChessObject>();

        private List<ChessObject> player = new List<ChessObject>();

        private RectangleObject[,] board = new RectangleObject[8, 8];
        private RectangleObject? check_cell = null;

        private byte[,] capture_board = new byte[8, 8];

        public bool[,] enemy_board = new bool[8, 8];
        public bool[,] player_board = new bool[8, 8];

        private ChessObject cur_piece;
        private int cur_pieceInd;

        private DisplayObject selected_move;

        protected DisplayObject[]? available_moves;

        King? player_king = null, enemy_king = null;
        private bool king_checked = false;
        public bool game_over = false;

        public WMPLib.WindowsMediaPlayer PlaySound = new WMPLib.WindowsMediaPlayer();

        public ChessBoard(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness, Color? fill = null) :
            base(topLeftX, topLeftY, bottomRightX, bottomRightY, borderThickness, fill)
        {

        }


        private void RemovePiece(int i, int j)
        {
            ChessObject? target = enemy.SingleOrDefault(x => x.i == i && x.j == j);
            if (target != null)
            {
                enemy.Remove(target);
                return;
            }

            target = player.SingleOrDefault(x => x.i == i && x.j == j);
            player.Remove(target);
        }

        private void FullUpdate()
        {

            for (int i = 0; i < player.Count; i++)
            {
                player[i].updateMoves(capture_board);
                if (!(player[i] is King))
                {
                    RemoveUnavailableMoves(player[i]);
                }

            }

            for (int i = 0; i < enemy.Count; i++)
            {
                if (enemy[i] is King)
                {
                    player[i].updateMoves(capture_board);
                }
                enemy[i].updateMoves(capture_board);
            }
            UpdateCaptureBoards();
        }

        private void RemoveUnavailableMoves(ChessObject cur)
        {
            bool[,] enemy_board = this.enemy_board.Clone() as bool[,];
            byte[,] capture_board = this.capture_board.Clone() as byte[,];

            for (int t = cur.Moves.Count - 1; t >= 0; t--)
            {
                List<ChessObject> enemy = new List<ChessObject>(this.enemy);
                var mv = cur.Moves[t];
                capture_board[cur.i, cur.j] = 0;
                if (capture_board[mv.Item1, mv.Item2] == 2)
                {
                    ChessObject target = enemy.Single(x => x.i == mv.Item1 && x.j == mv.Item2);
                    enemy.Remove(target);
                }
                capture_board[mv.Item1, mv.Item2] = 1;

                int king_ind = 0;
                for (int i = 0; i < enemy.Count; i++)
                {
                    if (enemy[i] is King)
                    {
                        (enemy[i] as King).updateMoves(capture_board);
                    }
                    enemy[i].updateMoves(capture_board);
                }

                Array.Clear(enemy_board, 0, enemy_board.Length);

                for (int i = 0; i < enemy.Count; i++)
                {
                    for (int j = 0; j < enemy[i].AttackedCells.Count; j++)
                    {
                        enemy_board[enemy[i].AttackedCells[j].Item1, enemy[i].AttackedCells[j].Item2] = true;
                    }
                }

                if (cur is King && enemy_board[mv.Item1, mv.Item2] == true)
                {
                    cur.Moves.RemoveAt(t);
                }
                else if (!(cur is King) && enemy_board[player_king.i, player_king.j] == true)
                {
                    cur.Moves.RemoveAt(t);
                }

                capture_board[mv.Item1, mv.Item2] = 0;
                capture_board[cur.i, cur.j] = 1;
            }
        }

        private void UpdateCaptureBoards()
        {
            Array.Clear(player_board, 0, player_board.Length);
            Array.Clear(enemy_board, 0, enemy_board.Length);

            for (int i = 0; i < player.Count; i++)
            {
                for (int j = 0; j < player[i].AttackedCells.Count; j++)
                {
                    player_board[player[i].AttackedCells[j].Item1, player[i].AttackedCells[j].Item2] = true;
                }
            }
            for (int i = 0; i < enemy.Count; i++)
            {
                for (int j = 0; j < enemy[i].AttackedCells.Count; j++)
                {
                    enemy_board[enemy[i].AttackedCells[j].Item1, enemy[i].AttackedCells[j].Item2] = true;
                }
            }

            player_king.updateMoves(capture_board);
            enemy_king.updateMoves(capture_board);
            RemoveUnavailableMoves(player_king);
        }

        private void PlayerMove()
        {
            ENDTURN = true;
            STARTI = 7 - cur_piece.i;
            STARTJ = cur_piece.j;
            DESTI = 7 - selected_move.anchorY / cur_piece.height;
            DESTJ = selected_move.anchorX / cur_piece.width;

            cur_piece.ShiftObject(selected_move.anchorX - cur_piece.anchorX, selected_move.anchorY - cur_piece.anchorY);
            capture_board[cur_piece.i, cur_piece.j] = 0;
            cur_piece.RenewPos();

            if (capture_board[cur_piece.i, cur_piece.j] == 2)
            {
                ChessObject? target = enemy.SingleOrDefault(x => x.i == cur_piece.i && x.j == cur_piece.j);
                enemy.Remove(target);
                PlaySound.URL = @"assets\sound\capture.mp3";
            }
            else
            {
                PlaySound.URL = @"assets\sound\move.mp3";
            }
            capture_board[cur_piece.i, cur_piece.j] = 1;

            if (cur_piece is Pawn)
            {
                if (cur_piece.i == 0)
                {
                    Queen queen = (cur_piece as Pawn).Promote();
                    ChessObject? target = player.SingleOrDefault(x => x == cur_piece);
                    player.Remove(target);
                    player.Add(queen);
                }
                else
                {
                    (cur_piece as Pawn).PawnMoved();
                }
            }
            FullUpdate();

            if (check_cell != null)
            {
                check_cell.ReverseColor();
                check_cell = null;
            }

            if (player_board[enemy_king.i, enemy_king.j] == true)
            {
                check_cell = board[enemy_king.i, enemy_king.j];
                check_cell.ReverseColor();
                PlaySound.URL = @"assets\sound\capture.mp3";
            }

        }

        public void EnemyMove(int i1, int j1, int i2, int j2)
        {
            ChessObject? target = enemy.SingleOrDefault(x => x.i == i1 && x.j == j1);
            target.ShiftObject((j2 - j1) * target.width, (i2 - i1) * target.height);

            if (capture_board[i2, j2] == 1)
            {
                ChessObject take = player.Single(x => x.i == i2 && x.j == j2);
                player.Remove(take);
                PlaySound.URL = @"assets\sound\capture.mp3";
            }
            else
            {
                PlaySound.URL = @"assets\sound\move.mp3";
            }

            capture_board[target.i, target.j] = 0;
            target.RenewPos();
            capture_board[target.i, target.j] = 2;

            if (target is Pawn)
            {
                if (target.i == 7)
                {
                    Queen queen = (target as Pawn).Promote();
                    player.Remove(target);
                    enemy.Add(queen);
                }
                else
                {
                    (target as Pawn).PawnMoved();
                }
            }
            FullUpdate();

            if (check_cell != null)
            {
                check_cell.ReverseColor();
                check_cell = null;
            }

            if (enemy_board[player_king.i, player_king.j] == true)
            {
                check_cell = board[player_king.i, player_king.j];
                check_cell.ReverseColor();
                king_checked = true;
                FullUpdate();
                bool res = CheckForMate();
                if (res == true)
                {
                    game_over = true;
                }
                PlaySound.URL = @"assets\sound\capture.mp3";
            }
        }

        public override bool Click(int mouseX, int mouseY)
        {
            ENDTURN = false;
            if (base.Click(mouseX, mouseY))
            {
                // transfer coordinates to relative to gamefield coordinates
                mouseX -= clientX1;
                mouseY -= clientY1;
                int i;
                bool clickHandled = false;
                if (available_moves != null)
                {
                    i = available_moves.Length - 1;
                    while (!clickHandled && i >= 0)
                    {
                        clickHandled = available_moves[i].Click(mouseX, mouseY);
                        i--;
                    }
                    if (clickHandled)
                    {
                        selected_move = available_moves[i + 1];
                        PlayerMove();

                        PlaySound.controls.play();

                    }
                    Array.Clear(available_moves);
                    available_moves = null;
                    if (clickHandled)
                    {
                        return true;
                    }
                }
                i = player.Count - 1;
                while (!clickHandled && i >= 0)
                {
                    clickHandled = player[i].Click(mouseX, mouseY);
                    i--;
                }
                if (clickHandled)
                {
                    cur_pieceInd = i + 1;
                    cur_piece = player[i + 1];
                    available_moves = player[i + 1].getMoves(ref capture_board);
                    return true;
                }
            }

            return false;
        }

        private bool CheckForMate()
        {
            foreach (ChessObject piece in player)
            {
                if (piece.Moves.Count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void InitArrays(RectangleObject[,] board, TextObject[] letters, List<ChessObject> enemy, List<ChessObject> player)
        {
            this.letters = letters;
            this.enemy = enemy;
            this.player = player;
            this.board = board;
            for (int i = 0; i < player.Count; i++)
            {
                capture_board[player[i].i, player[i].j] = 1;
            }
            for (int i = 0; i < enemy.Count; i++)
            {
                capture_board[enemy[i].i, enemy[i].j] = 2;
            }

            player_king = (King)player.Single(x => x is King);
            enemy_king = (King)enemy.Single(x => x is King);

            //starting moves
            FullUpdate();
        }


        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            g.TranslateTransform(fieldX1, fieldY1);

            // Drawing the outer frame
            Rectangle rectOuter = new Rectangle(0, 0, fieldX2 - fieldX1, fieldY2 - fieldY1);
            g.FillRectangle(strokeBrush, rectOuter);
            g.DrawRectangle(new Pen(strokeBrush), rectOuter);

            // Drawing the client area            
            g.TranslateTransform(clientX1 - frameX1, clientY1 - frameY1);
            Rectangle rectClient = new Rectangle(0, 0, clientX2 - clientX1, clientY2 - clientY1);
            g.FillRectangle(fillBrush, rectClient);
            g.DrawRectangle(new Pen(fillBrush), rectClient);


            // drawing the objects inside the field
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].Draw(g);
                }
            }

            for (int i = 0; i < 16; i++)
            {
                letters[i].Draw(g);
            }

            for (int i = 0; i < player.Count; i++)
            {
                player[i].Draw(g);
            }

            for (int i = 0; i < enemy.Count; i++)
            {
                enemy[i].Draw(g);
            }

            if (available_moves != null)
            {
                for (int i = 0; i < available_moves.Length; i++)
                {
                    available_moves[i].Draw(g);
                }
            }
        }

    }
}
