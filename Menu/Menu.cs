using DisplayObjects;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuObjects
{
    internal class Menu : DisplayObject
    {

        protected int Max_Items = 11;
        protected int itemCount;
        public MenuItem[] items;
        public MenuItem lastClickedItem;

        public int offsetX;
        public int req_offsetX;

        public int offsetY;
        public int req_offsetY;

        public Menu(int x, int y) : 
            base(x, y, 0, 0)
        {
            // x and y are anchor points defining top left 
            // corner, relative to which the items are placed
            // in the menu
            items = new MenuItem[Max_Items];
            itemCount = 0;

            offsetX = 100;
            offsetY = 0;
        }

        public override bool Click(int mouseX, int mouseY)
        {
            // menu items are always visible. just check each of them  
            int i = itemCount - 1;
            bool clickHandled = false;
            while (!clickHandled && i >= 0) {
                clickHandled = items[i].Click(mouseX, mouseY);
                i--;
            }

            // if some of the clicks has been processed,
            // hide all other menu elements if they're visible
            if (clickHandled)
            {
                i++;
                for (int j = 0; j < itemCount; j++)
                {
                    if (i == j) continue;
                    //if (items[j].subMenuVisible)
                    //{
                    items[j].subMenuVisible = false;
                    //}
                }
                lastClickedItem = items[i];
            }
            else {
                lastClickedItem = null;
            }
            return clickHandled;            
        }

        public int? AddItem(MenuItem item)
        {

            if (itemCount != Max_Items)
            {
                // add the item, including its offset
                // shift the coordinates according to the last item
                int topLeftX = anchorX;
                int topLeftY = anchorY;

                if (itemCount != 0) {
                    topLeftX = items[itemCount - 1].frameX1 + offsetX;
                    topLeftY = items[itemCount - 1].frameY1 + offsetY;
                }

                // shift coords according to the new point
                item.ShiftObject(topLeftX - item.frameX1,  topLeftY- item.frameY1);

                items[itemCount] = item;
                return itemCount++;
            }
            return null;
        }

        public bool DeleteItem(int index)
        {
            if (index >= 0 && index < itemCount)
            {
                for (int i = index; i < itemCount - 1; i++)
                {
/*                    items[i + 1].ShiftObject(items[i].frameX2 - items[i].frameX1, 
                        items[i].frameY2 - items[i].frameY1);*/
                    items[i] = items[i + 1];
                }
                itemCount--;
                return true;
            }
            return false;

        }
        public bool DeleteItem(MenuItem obj)
        {
            int i = 0;
            bool flagFound = false;
            while (i < itemCount && !flagFound)
            {
                flagFound = items[i] == obj;
                i++;
            }
            if (flagFound)
            {
                i--;
                return DeleteItem(i);
            }
            else
            {
                return false;
            }
        }

        public override void Draw(Graphics g)
        {
            for (int i = 0; i < itemCount; i++)
            {
                items[i].Draw(g);
            }
        }

        protected override void ShiftCoords(int deltaX, int deltaY) { }

    }
}
