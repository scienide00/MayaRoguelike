﻿using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Maya
{
    public sealed class Window
    {
        private static List<Window> activeWindows = new List<Window>();
        private Unit pc;
        private Coordinate topLeft, topRight, bottomLeft, bottomRight;  //window frame coordinates!
        private string title;
        private bool windowIsOpen = false;
        private int windowBottomLeftX = 0;
        private int windowBottomLeftY = Globals.CONSOLE_HEIGHT - (Globals.CONSOLE_HEIGHT - Globals.GAME_FIELD_BOTTOM_RIGHT.Y);
        private int windowWidth = Globals.GAME_FIELD_BOTTOM_RIGHT.X;
        private int windowHeight = Globals.CONSOLE_HEIGHT - (Globals.CONSOLE_HEIGHT - Globals.GAME_FIELD_BOTTOM_RIGHT.Y);
        private int windowMargin = 3;
        private int linePosition;

        /// <summary>
        /// Default constructor for creating a window over the game field.
        /// </summary>
        /// <param name="pc">Information for this unit is used in the window.</param>
        /// <param name="title">Window title.</param>
        public Window(Unit pc, string title)
        {
            this.pc = pc;

            this.title = title.ToUpper();

            //save window coordinates
            this.bottomLeft = new Coordinate((windowBottomLeftX + windowMargin) - 1, windowBottomLeftY - windowMargin);
            this.topLeft = new Coordinate((windowHeight - windowBottomLeftY) + windowMargin - 1, windowBottomLeftX + windowMargin);
            this.bottomRight = new Coordinate((windowBottomLeftX + windowWidth) - windowMargin, windowBottomLeftY - windowMargin);
            this.topRight = new Coordinate((windowBottomLeftX + windowWidth) - windowMargin, (windowHeight - windowBottomLeftY) + windowMargin);
            this.linePosition = this.TopLeft.Y + 2;
            activeWindows.Add(this);
        }
        
        /// <summary>
        /// Constructor for manually setting the size/position of the window.
        /// </summary>
        /// <param name="pc">Information for this unit is used in the window.</param>
        /// <param name="title">Window title.</param>
        /// <param name="windowBottomLeftX">Bottom-left window corner 'x' coordinate.</param>
        /// <param name="windowBottomLeftY">Bottom-left window corner 'y' coordinate.</param>
        public Window(Unit pc, string title, int windowBottomLeftX, int windowBottomLeftY, int windowWidth, int windowHeight)
        {
            this.pc = new Unit(pc);
            this.title = title.ToUpper();

            if (windowBottomLeftX <= Globals.CONSOLE_WIDTH && windowBottomLeftX >= 0)
            {
                this.windowBottomLeftX = windowBottomLeftX;
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("windowBottomLeftX = {0}. windowBottomLeftX should be in range 0 - {1}.",
                    windowBottomLeftX, Globals.CONSOLE_WIDTH));
            }

            if (windowBottomLeftY <= Globals.CONSOLE_HEIGHT && windowBottomLeftY > 0)
            {
                this.windowBottomLeftY = windowBottomLeftY;
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("windowBottomLeftY = {0}. windowBottomLeftY should be in range 0 - {1}.",
                    windowBottomLeftY, Globals.CONSOLE_HEIGHT));
            }

            if (windowWidth <= Globals.CONSOLE_WIDTH && windowWidth >= 0)
            {
                this.windowWidth = windowWidth;
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("windowWidth = {0}. windowWidth should be in range 0 - {1}.",
                    windowWidth, Globals.CONSOLE_WIDTH));
            }

            if (windowHeight <= Globals.CONSOLE_HEIGHT && windowHeight > 0)
            {
                this.windowHeight = windowHeight;
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("windowHeight = {0}. windowHeight should be in range 0 - {1}.",
                    windowHeight, Globals.CONSOLE_HEIGHT));
            }

            this.linePosition = this.TopLeft.Y + 1;

            activeWindows.Add(this);
        }

        #region WindowSize&Coordinates
        public Coordinate TopLeft
        {
            get { return this.topLeft; }
        }

        public Coordinate TopRight
        {
            get { return this.topRight; }
        }
        
        public Coordinate BottomLeft
        {
            get { return this.bottomLeft; }
        }

        public Coordinate BottomRight
        {
            get { return this.bottomRight; }
        }

        public Coordinate BottomLeftAnchor
        {
            get { return new Coordinate(this.windowBottomLeftX, windowBottomLeftY); }
        }

        public int WindowWidth
        {
            get { return this.windowWidth; }
        }

        public int WindowHeight
        {
            get { return this.windowHeight; }
        }

        public int WindowMargin
        {
            get { return this.windowMargin; }
        }
        #endregion

        public static List<Window> ActiveWindows
        {
            get { return activeWindows; }
            set { activeWindows = value; }
        }

        public string Title
        {
            get { return this.title; }
        }

        public void Show()
        {
            if (!windowIsOpen)
            {
                ConsoleTools.Clear(windowBottomLeftX, windowBottomLeftY, windowWidth, windowHeight);
                DrawWindow();
                this.windowIsOpen = true;
                GameEngine.MessageLog.SendMessage(string.Format("\"{0}\" opened.", title));
            }
        }

        public bool Write(string text, ConsoleColor color = ConsoleColor.Gray, int line = 1)
        {
            if (!(linePosition >= BottomLeft.Y - 2))
            {
                if (this.TopLeft.X + text.Length <= this.TopRight.X - 2)
                {
                    linePosition += line;
                    ConsoleTools.WriteOnPosition(text, (windowHeight - windowBottomLeftY) + (windowMargin + 2),
                        windowBottomLeftX + (linePosition - 1), color);
                    return true;
                }
                    //else block copied from MessageLog.SendMessage
                else
                {
                    string[] splitText = text.Split(' ');
                    int firstUnappendedString = 0;
                    int width = this.TopRight.X - this.TopLeft.X;
                    StringBuilder firstPartText = new StringBuilder(width);

                    for (int i = firstUnappendedString; i < splitText.Length; i++)
                    {
                        if (!(firstPartText.Length + (splitText[i].Length + 1) > width - 2))
                        {
                            firstPartText.Append(splitText[i]);
                            firstPartText.Append(" ");
                        }
                        else
                        {
                            firstUnappendedString = i;
                            break;
                        }
                    }

                    StringBuilder secondPartText = new StringBuilder(width);
                    for (int i = firstUnappendedString; i < splitText.Length; i++)
                        secondPartText.AppendFormat("{0} ", splitText[i]);

                    Write(firstPartText.ToString());
                    Write(secondPartText.ToString());
                    return true;
                }
            }
            return false;
        }

        public bool WriteLine(string str, ConsoleColor color = ConsoleColor.Gray)
        {
            return Write(str, color, 2);
        }

        public void CloseWindow()
        {
            ActiveWindows.Remove(this);
            this.windowIsOpen = false;
            ConsoleTools.Clear(windowBottomLeftX, windowBottomLeftY, windowWidth, windowHeight);
            GameEngine.VisualEngine.PrintFOVMap(pc.X, pc.Y);
            GameEngine.VisualEngine.PrintUnit(pc);
        }

        public void ResetLinePosition()
        {
            this.linePosition = this.TopLeft.Y + 2;
            var emptySB = new StringBuilder(windowWidth - 8);
            emptySB.Append(new string(' ', windowWidth - 8));
            while (this.Write(emptySB.ToString()));
            this.linePosition = this.TopLeft.Y + 2;
        }

        //draw the window over the game field
        private void DrawWindow()
        {
            //bottom left corner
            ConsoleTools.WriteOnPosition('\u255a', (windowBottomLeftX + windowMargin) - 1, windowBottomLeftY - windowMargin, ConsoleColor.Cyan);

            //bottom side
            for (int y = windowBottomLeftX + windowMargin; y < (windowBottomLeftX + windowWidth) - windowMargin; y++)
                ConsoleTools.WriteOnPosition('\u2550', y, windowBottomLeftY - windowMargin, ConsoleColor.Cyan);

            //bottom right corner
            ConsoleTools.WriteOnPosition('\u255d', (windowBottomLeftX + windowWidth) - windowMargin, windowBottomLeftY - windowMargin, ConsoleColor.Cyan);

            //right side
            for (int x = windowBottomLeftY - (windowMargin + 1); x >= windowMargin + 1; x--)
                ConsoleTools.WriteOnPosition('\u2551', (windowBottomLeftX + windowWidth) - windowMargin, x, ConsoleColor.Cyan);

            //top right corner
            ConsoleTools.WriteOnPosition('\u2557', (windowBottomLeftX + windowWidth) - windowMargin, (windowHeight - windowBottomLeftY) + windowMargin, ConsoleColor.Cyan);

            //top side
            for (int y = (windowBottomLeftX + windowWidth) - (windowMargin + 1); y >= (windowBottomLeftY - windowHeight) + windowMargin; y--)
                ConsoleTools.WriteOnPosition('\u2550', y, (windowHeight - windowBottomLeftY) + windowMargin, ConsoleColor.Cyan);

            //top left corner
            ConsoleTools.WriteOnPosition('\u2554', (windowHeight - windowBottomLeftY) + windowMargin - 1, windowBottomLeftX + windowMargin, ConsoleColor.Cyan);

            //left side
            for (int x = (windowBottomLeftY - windowHeight) + windowMargin + 1; x < windowBottomLeftY - windowMargin; x++)
                ConsoleTools.WriteOnPosition('\u2551', windowBottomLeftX + windowMargin - 1, x, ConsoleColor.Cyan);

            //write window title
            StringBuilder SBTitle = new StringBuilder();
            for (int i = 0; i < title.Length; i++)
            {
                SBTitle.Append(title[i]);
                SBTitle.Append(' ');
            }

            int titleX = (windowWidth - SBTitle.Length) / 2;
            int titleY = ((windowHeight - windowBottomLeftY) + windowMargin) - 1;

            ConsoleTools.WriteOnPosition(SBTitle.ToString(), titleX, titleY, ConsoleColor.Yellow);
        }
    }
}
       