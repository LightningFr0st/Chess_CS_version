using Chess;
using System.Drawing.Drawing2D;

namespace DisplayObjects
{
    internal class RectangleObject : DisplayObject
    {
        protected ChessObject cur_piece;

        // Rectangle coordinates
        protected int rectX1, rectY1, rectX2, rectY2;

        public int i, j;

        // Initializer for all constructors of rectangle
        private void InitializeCoords(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
        {
            rectX1 = topLeftX;
            rectY1 = topLeftY;
            rectX2 = bottomRightX;
            rectY2 = bottomRightY;
            i = anchorY / width;
            j = anchorX / height;
            //clickHandler = () =>
            //{
            //    Console.WriteLine("Tile clicked! Coords:{0},{1}",anchorX, anchorY);
            //};
        }

        // Constructor for rectangle with fill color
        public RectangleObject(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, Color? fill = null) :
        base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2, bottomRightX - topLeftX, bottomRightY - topLeftY, fill)
        {
            InitializeCoords(topLeftX, topLeftY, bottomRightX, bottomRightY);
        }

        // Constructor for rectangle with texture filling
        public RectangleObject(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, Bitmap bmp) :
        base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2, bottomRightX - topLeftX, bottomRightY - topLeftY, bmp)
        {
            InitializeCoords(topLeftX, topLeftY, bottomRightX, bottomRightY);
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();
            Rectangle rect = new Rectangle((int)rectX1, (int)rectY1, (int)(rectX2 - rectX1), (int)(rectY2 - rectY1));
            if (fillBrush is TextureBrush)
            {
                (fillBrush as TextureBrush).TranslateTransform(rectX1, rectY1);
            }
            g.FillRectangle(fillBrush, rect);
            if (strokeThickness > 0)
            {
                g.DrawRectangle(new Pen(strokeBrush, strokeThickness), rect);
            }

            //g.DrawRectangle(new Pen(strokeBrush, 3), rect);
        }

        protected override void ShiftCoords(int deltaX, int deltaY)
        {
            rectX1 += deltaX;
            rectY1 += deltaY;
            rectX2 += deltaX;
            rectY2 += deltaY;
        }
    }
}
