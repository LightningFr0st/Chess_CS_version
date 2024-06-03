namespace DisplayObjects
{
    internal class CircleObject : EllipseObject
    {

        // Constructor for circle with fill color
        public CircleObject(int centerX, int centerY, int radius, Color? fill = null) :
            base(centerX, centerY, radius, radius, fill)
        { }

        // Constructor for circle with fill texture
        public CircleObject(int centerX, int centerY, int radius, Bitmap bmp) :
            base(centerX, centerY, radius, radius, bmp)
        { }
    }
}
