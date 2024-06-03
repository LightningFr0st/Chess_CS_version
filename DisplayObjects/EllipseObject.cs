
using System.Drawing.Drawing2D;
using System.Security.Cryptography.Xml;
using Utilities;

namespace DisplayObjects
{
    internal class EllipseObject : DisplayObject
    {
        // radius properties of the ellipse
        protected int radiusX, radiusY;

        // ellipse center coordinates
        protected int centerX, centerY;

        // Initializer for all constructors of ellipse
        private void InitializeCoords(int centerCoordX, int centerCoordY, int radX, int radY)
        {
            centerX = centerCoordX;
            centerY = centerCoordY;
            radiusX = radX;
            radiusY = radY;
        }

        // Constructor for ellipse with fill color
        public EllipseObject(int centerX, int centerY, int radiusX, int radiusY, Color? fill = null) :
        base(centerX, centerY, radiusX * 2, radiusY * 2, fill)
        {
            InitializeCoords(centerX, centerY, radiusX, radiusY);
        }

        // Constructor for ellipse with texture filling
        public EllipseObject(int centerX, int centerY, int radiusX, int radiusY, Bitmap bmp) :
        base(centerX, centerY, radiusX * 2, radiusY * 2, bmp)
        {
            InitializeCoords(centerX, centerY, radiusX, radiusY);
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            Rectangle ellipse = new Rectangle(centerX - radiusX, centerY - radiusY, radiusX * 2, radiusY * 2);
            g.FillEllipse(fillBrush, ellipse);
            if (strokeThickness > 0)
            {
                g.DrawEllipse(new Pen(strokeBrush, strokeThickness), ellipse);
            }
        }

        protected override void ShiftCoords(int deltaX, int deltaY)
        {
            centerX += deltaX;
            centerY += deltaY;
        }
    }
}
