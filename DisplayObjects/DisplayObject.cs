using System.Drawing.Drawing2D;
using Utilities;

namespace DisplayObjects
{
    public abstract class DisplayObject
    {
        // frame that surrounds the drawn rotated object 
        public int frameX1;
        public int frameY1;
        public int frameX2;
        public int frameY2;

        // anchor point (center of the shape)
        public int anchorX;
        public int anchorY;

        // colors of fill of the shape
        protected Color _fillColor;
        private Bitmap _fillBMP;
        private bool colorFill;
        public Color fillColor {
            get { 
                return _fillColor; 
            }
            set {
                _fillColor = value;
                colorFill = true;    
            } 
        }
        public Bitmap fillBMP
        {
            get {
                return _fillBMP;
            }
            set {
                _fillBMP = value;
                colorFill = false;
            }
        }

        public (int,int) GetSize
        {
            get
            {
                return (width, height);
            }
        }

        // colors of stroke of the shape
        public Color borderColor;
        public int strokeThickness;

        public int width;
        public int height;

        // current trigger state of DisplayObject
        protected bool isClicked;
        public delegate void ClickHandler();

        public ClickHandler? clickHandler;

        // Click check on the DisplayObject
        public virtual bool Click(int mouseX, int mouseY)
        {
            isClicked = (mouseX >= frameX1) && (mouseX <= frameX2) &&
                (mouseY >= frameY1) && (mouseY <= frameY2);
            if (isClicked)
            {
                clickHandler?.Invoke();
            }
            return isClicked;            
        }


        // Constructor for a DisplayObject with default color
        private DisplayObject(int anchX, int anchY, int objWidth, int objHeight, Color? color, Bitmap bmp)
        {
            anchorX = anchX;
            anchorY = anchY;

            frameX1 = anchX - objWidth / 2;
            frameY1 = anchY - objHeight / 2;
            frameX2 = frameX1 + objWidth;
            frameY2 = frameY1 + objHeight;

            width = frameX2 - frameX1;
            height = frameY2 - frameY1; 

            isClicked = false;
            clickHandler = null;

            if (color != null)
            {
                fillColor = color.Value;
            }
            else if (bmp != null)
            {
                fillBMP = bmp;
            }
            else { 
                fillColor = Color.Black;
            }

            borderColor = Color.Black;
            strokeThickness = 1;
        }

        private Color save = Color.Red;
        public void ReverseColor()
        {
            Color temp = fillColor;
            fillColor = save;
            save = temp;
        }

        // Constructor for DisplayObject with fill color
        public DisplayObject(int anchX, int anchY, int objWidth, int objHeight, Color? fill = null) :
            this(anchX, anchY, objWidth, objHeight, fill, null)
        {
        }

        // Constructor for DisplayObject with fill texture
        public DisplayObject(int anchX, int anchY, int objWidth, int objHeight, Bitmap bmp) :
            this(anchX, anchY, objWidth, objHeight, null, bmp)
        {
        }

        // Setting the stroke thickness
        public void SetStrokeThickness(int thickness)
        {
            strokeThickness = thickness;
        }

        // Acquiring relevant fill brush for drawing the object
        protected Brush GetFillBrush()
        {
            if (colorFill)
                return new SolidBrush(fillColor);
            else
                return new TextureBrush(fillBMP);
        }

        // Acquiring relevant stroke brush for drawing the object
        protected Brush GetStrokeBrush()
        {
            return new SolidBrush(borderColor);
        }

        // Each method must implement Draw method using 
        public abstract void Draw(Graphics g);

        // Check if the object is in the bounds of a frame
        public bool IsInBounds(int borderX1, int borderY1, int borderX2, int borderY2)
        {
            return
                frameX1 > borderX1 &&
                frameY1 > borderY1 &&
                frameX2 < borderX2 &&
                frameY2 < borderY2;
        }

        //shift main coordinates
        public void ShiftObject(int deltaX, int deltaY)
        {
            anchorX += deltaX;
            anchorY += deltaY;

            frameX1 += deltaX;
            frameY1 += deltaY;
            frameX2 += deltaX;
            frameY2 += deltaY;
            ShiftCoords(deltaX, deltaY);
        }

        // shift additional coordinates
        protected abstract void ShiftCoords(int deltaX, int deltaY);

    }
}