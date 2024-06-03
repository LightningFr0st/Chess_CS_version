using Utilities;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayObjects
{
    internal class TextObject:DisplayObject
    {
        public string textData;
        public FontFamily fontFamily;
        public int fontSize;

        public int boxX1, boxY1, boxX2, boxY2;

        public TextObject(string text, int frameX1, int frameY1, int frameX2, int frameY2, FontFamily? textFontFamily, int? textFontSize, Color? fill) : 
            base((frameX1+frameX2)/2, (frameY1 + frameY2) / 2, (frameX2 - frameX1), (frameY2-frameY1), fill)
        {
            textData = text;
            boxX1 = frameX1; boxY1 = frameY1;
            boxX2 = frameX2; boxY2 = frameY2;
            fontFamily = textFontFamily == null ? FontFamily.GenericMonospace : textFontFamily;
            fontSize = textFontSize == null ? 14 : textFontSize.Value;
        }


        public override void Draw(Graphics g)
        {
            Brush fillBrush = GetFillBrush();

            RectangleF box = new RectangleF(boxX1,boxY1,boxX2-boxX1,boxY2-boxY1);
            Font font = new Font(fontFamily, fontSize);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            drawFormat.LineAlignment = StringAlignment.Center;

            g.DrawString(textData, font, fillBrush, box, drawFormat);

            //Brush borderBursh = GetStrokeBrush();
            //Rectangle rect = Rectangle.Round(box);
            //g.DrawRectangle(new Pen(borderBursh, 2), rect);

        }

        protected override void ShiftCoords(int deltaX, int deltaY)
        {
            boxX1 += deltaX;
            boxY1 += deltaY;
            boxX2 += deltaX;
            boxY2 += deltaY;
        }

    }
}
