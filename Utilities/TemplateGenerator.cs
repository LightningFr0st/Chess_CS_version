using Microsoft.VisualBasic;
using DisplayObjects;
using MenuObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Utilities
{
    internal class TemplateGenerator
    {
        public enum Primitive
        {
            P_Rectangle,
            P_Square,
            P_Ellipse,
            P_Circle
        }

        public enum SubMenuDirection
        {
            Dir_Default,
            Dir_Horizontal,
            Dir_Vertical,
            Dir_Ladder
        }

        public Primitive activePrimitive;
        public int templateSizeX;
        public int templateSizeY;
        public Color viewFill;
        public Color viewStroke;
        public int viewStrokeThickness;

        public static int reqX, reqY, reqXt, reqYt;

        public SubMenuDirection direction;
        public static SubMenuDirection MainMenuDirection;
        public int offsetX;
        public int offsetY;

        public FontFamily fontFamily;
        public Color fontColor;
        public int fontSize;
        public string textData;


        public TemplateGenerator(
            Primitive? primitive = null, int? sizeX = null, int? sizeY = null,
            Color? vFill = null, Color? vStroke = null, int? vStrokeTh = null,
            Color? fColor = null, int? fSize = null
            )
        {
            activePrimitive = primitive == null ? Primitive.P_Rectangle : primitive.Value;
            templateSizeX = sizeX == null ? 100 : sizeX.Value;
            templateSizeY = sizeY == null ? 100 : sizeY.Value;
            viewFill = vFill == null ? Color.Red : vFill.Value;
            viewStroke = vStroke == null ? Color.Black : vStroke.Value;
            viewStrokeThickness = vStrokeTh == null ? 1 : vStrokeTh.Value;

            direction = SubMenuDirection.Dir_Vertical;
            offsetX = 0;
            offsetY = 0;

            fontFamily = FontFamily.GenericMonospace;
            fontColor = fColor == null ? Color.Black : fColor.Value;
            fontSize = fSize == null ? 14 : fSize.Value;
            textData = "";
        }

        public TemplateGenerator TemplateFromMenuItem(MenuItem item)
        {
            TemplateGenerator tg = new TemplateGenerator();
            DisplayObject display = item.view;
            TextObject text = item.text;
            tg.templateSizeX = display.frameX2 - display.frameX1;
            tg.templateSizeY = display.frameY2 - display.frameY1;
            tg.activePrimitive = Primitive.P_Rectangle;
            tg.viewFill = display.fillColor;
            tg.viewStroke = display.borderColor;
            tg.viewStrokeThickness = display.strokeThickness;
            tg.fontColor = text.fillColor;
            tg.fontSize = text.fontSize;
            tg.textData = text.textData;
            return tg;
        }


        public (int, int) GenerateOffsets(int reqX, int reqY, SubMenuDirection dir_p)
        {
            int offsX = 0, offsY = 0;
            switch (dir_p)
            {
                case (SubMenuDirection.Dir_Vertical):
                    offsX = 0;
                    offsY = templateSizeY + reqY;
                    break;

                case (SubMenuDirection.Dir_Horizontal):
                    offsX = templateSizeX + reqX;
                    offsY = 0;
                    break;

                case (SubMenuDirection.Dir_Ladder):
                    offsX = templateSizeX + reqX;
                    offsY = templateSizeY + reqY;
                    break;

                case (SubMenuDirection.Dir_Default):
                default:
                    offsX = reqX;
                    offsY = reqY;
                    break;
            }
            return (offsX, offsY);
        }

        public MenuItem GetTemplatePrimitive(string? textData = null)
        {
            textData = textData == null ? this.textData : textData;
            int frameX1 = -(templateSizeX / 2);
            int frameY1 = -(templateSizeY / 2);
            int frameX2 = (templateSizeX / 2);
            int frameY2 = (templateSizeY / 2);

            DisplayObject displayObject;
            TextObject text = new TextObject(textData, frameX1, frameY1, frameX2, frameY2,fontFamily, fontSize, fontColor);
            switch (activePrimitive)
            {
                case Primitive.P_Rectangle:
                    displayObject = new RectangleObject(frameX1, frameY1, frameX2, frameY2, viewFill);
                    break;
                case Primitive.P_Square:
                    displayObject = new SquareObject(frameX1, frameY1, templateSizeX, viewFill);
                    break;
                case Primitive.P_Ellipse:
                    displayObject = new EllipseObject(0, 0, templateSizeX / 2, templateSizeY / 2, viewFill);
                    break;
                case Primitive.P_Circle:
                    displayObject = new CircleObject(0, 0, templateSizeX / 2, viewFill);
                    break;
                default:
                    displayObject = new RectangleObject(frameX1, frameY1, frameX2, frameY2, viewFill);
                    break;
            }
            displayObject.SetStrokeThickness(viewStrokeThickness);
            displayObject.borderColor = viewStroke;

            MenuItem item = new MenuItem(displayObject, text);

            item.offsetX = this.offsetX;
            item.offsetY = this.offsetY;

            return item;
        }

    }
}
