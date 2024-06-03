using DisplayObjects;
using MenuObjects;
using Utilities;
using System.Diagnostics;
using Microsoft.VisualBasic;
using static Utilities.TemplateGenerator;

namespace oop2
{
    public partial class Chess : Form
    {

        private static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private static int FPS = 60;
        private static int timeInterval = 1000 / FPS;

        private static Game MainGame;

        public Chess()
        {
            InitializeComponent();

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            WindowState = FormWindowState.Normal;
            this.ClientSize = new Size(400, 400);
            Rectangle workArea = Screen.GetWorkingArea(this);

            MainGame = new Game();
            MainGame.game_form = this;

            timer.Tick += new EventHandler(TimerHandler);
            timer.Interval = timeInterval;
            timer.Enabled = true;
        }

        private void Chess_Paint(object sender, PaintEventArgs e)
        {
            MainGame.DrawGame(e.Graphics);
        }

        public void RenewSize((int, int) w_h)
        {
            int fieldWidth, fieldHeight;
            (fieldWidth, fieldHeight) = w_h;
            this.ClientSize = new Size(fieldWidth, fieldHeight);
            this.CenterToScreen();

        }

        private void TimerHandler(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Chess_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
            MainGame.HandleClick(x, y);
        }
    }
}
