using Chess;
using MenuObjects;
using System;
using System.Drawing.Drawing2D;
using System.Media;
namespace DisplayObjects
{
    internal class GameField : DisplayObject
    {
        // outer frame coordinates
        public int fieldX1, fieldY1;
        public int fieldX2, fieldY2;

        // client field coordinates
        public int clientX1, clientY1;
        public int clientX2, clientY2;


        // gamefield objects storage
        private readonly int MaxObjects = 112;

        private DisplayObject[] objects;
        public Menu? main_menu = null;

        private int objCount;

        // Initializer for all constructors of the DrawField
        private void InitializeFields(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness)
        {

            fieldX1 = topLeftX;
            fieldY1 = topLeftY;
            fieldX2 = bottomRightX;
            fieldY2 = bottomRightY;

            clientX1 = fieldX1 + borderThickness;
            clientY1 = fieldY1 + borderThickness;
            clientX2 = fieldX2 - borderThickness;
            clientY2 = fieldY2 - borderThickness;

            objects = new DisplayObject[MaxObjects];
            objCount = 0;
        }


        // Constructor for field with fill color
        public GameField(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness, Color? fill = null) :
            base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2,
                bottomRightX - topLeftX, bottomRightY - topLeftY, fill)
        {
            InitializeFields(topLeftX, topLeftY, bottomRightX, bottomRightY, borderThickness);
        }

        // Constructor for field with fill texture
        public GameField(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness, Bitmap bmp) :
            base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2,
                bottomRightX - topLeftX, bottomRightY - topLeftY, bmp)
        {
            InitializeFields(topLeftX, topLeftY, bottomRightX, bottomRightY, borderThickness);
        }

        public override bool Click(int mouseX, int mouseY)
        {
            if (base.Click(mouseX, mouseY))
            {
                // transfer coordinates to relative to gamefield coordinates
                mouseX -= clientX1;
                mouseY -= clientY1;

                int i = objCount - 1;
                bool clickHandled = false;

                if (main_menu != null)
                {
                    clickHandled = main_menu.Click(mouseX, mouseY);
                    if (clickHandled) return true;
                }
                while (!clickHandled && i >= 0)
                {
                    clickHandled = objects[i].Click(mouseX, mouseY);
                    i--;
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        public int? AddObject(DisplayObject obj)
        {
            if (objCount != MaxObjects)
            {
                objects[objCount] = obj;
                return objCount++;
            }
            return null;
        }

        public bool DeleteObject(int index)
        {
            if (index < objCount)
            {
                for (int i = index; i < objCount - 1; i++)
                {
                    objects[i] = objects[i + 1];
                }
                objCount--;
                return true;
            }
            return false;

        }
        public bool DeleteObject(DisplayObject obj)
        {
            int i = 0;
            bool flagFound = false;
            while (i < objCount && !flagFound)
            {
                flagFound = objects[i] == obj;
                i++;
            }
            if (flagFound)
            {
                return DeleteObject(i);
            }
            else
            {
                return false;
            }
        }


        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();
            g.TranslateTransform(fieldX1, fieldY1);

            // Drawing the outer frame
            Rectangle rectOuter = new Rectangle(0, 0, (int)(fieldX2 - fieldX1), (int)(fieldY2 - fieldY1));
            g.FillRectangle(strokeBrush, rectOuter);
            g.DrawRectangle(new Pen(strokeBrush), rectOuter);

            // Drawing the client area            
            g.TranslateTransform(clientX1 - frameX1, clientY1 - frameY1);
            Rectangle rectClient = new Rectangle(0, 0, (int)(clientX2 - clientX1), (int)(clientY2 - clientY1));
            g.FillRectangle(fillBrush, rectClient);
            g.DrawRectangle(new Pen(fillBrush), rectClient);


            // drawing the objects inside the field
            for (int i = 0; i < objCount; i++)
            {
                objects[i].Draw(g);
            }
            if (main_menu != null)
            {
                main_menu.Draw(g);
            }
        }

        protected override void ShiftCoords(int deltaX, int deltaY)
        {
            fieldX1 += deltaX;
            fieldY1 += deltaY;
            fieldX2 += deltaX;
            fieldY2 += deltaY;

            clientX1 += deltaX;
            clientY1 += deltaY;
            clientX2 += deltaX;
            clientY2 += deltaY;
        }

    }
}
