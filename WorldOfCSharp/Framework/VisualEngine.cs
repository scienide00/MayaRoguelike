﻿using System;
using WorldOfCSharp.FieldOfView;

namespace WorldOfCSharp
{
    public class VisualEngine
    {
        private FieldOfView<GameCell> fieldOfView;
        private int range = 12;
        private FOVMethod method = FOVMethod.MRPAS;
        private RangeLimitShape shape = RangeLimitShape.Circle;
        private GameCell[,] map;
        private char[] itemCharacters;

        private int xStart;
        private int xEnd;
        private int yStart;
        private int yEnd;

        public VisualEngine(GameCell[,] map, int range, FOVMethod method, RangeLimitShape shape)
        {
            this.fieldOfView = new FieldOfView<GameCell>(map);
            this.range = range;
            this.method = method;
            this.shape = shape;
            this.map = new GameCell[0, 0];
            this.map = map;

            this.itemCharacters = new char[Enum.GetNames(typeof(BaseType)).Length];
            this.itemCharacters[(int)BaseType.Armor] = '[';
            this.itemCharacters[(int)BaseType.Weapon] = ')';
            this.itemCharacters[(int)BaseType.Consumable] = '%';
            this.itemCharacters[(int)BaseType.Container] = '&';
            this.itemCharacters[(int)BaseType.Gem] = '\u263c';
            this.itemCharacters[(int)BaseType.Key] = '\u2552';
            this.itemCharacters[(int)BaseType.Money] = '$';
            this.itemCharacters[(int)BaseType.Reagent] = '\u220f';
            this.itemCharacters[(int)BaseType.Recipe] = '\u222b';
            this.itemCharacters[(int)BaseType.Projectile] = '(';
            this.itemCharacters[(int)BaseType.Quest] = '\u2021';
            this.itemCharacters[(int)BaseType.Quiver] = '\u00b6';
            this.itemCharacters[(int)BaseType.TradeGoods] = '\u2211';
            this.itemCharacters[(int)BaseType.Miscellaneous] = '}';
        }

        public VisualEngine(GameCell[,] map)
            : this(map, 12, FOVMethod.MRPAS, RangeLimitShape.Octagon)
        { }

        public void PrintFOVMap(int FOV_X, int FOV_Y)
        {
            fieldOfView.ComputeFov(FOV_X, FOV_Y, this.range, true, this.method, this.shape);
            
            //calc print coords, for faster print loop
            const int PRINT_MARGINS = 2;
            if (FOV_X - (range + PRINT_MARGINS) >= 0)
                xStart = FOV_X - (range + PRINT_MARGINS);
            else
                xStart = 0;

            if (FOV_X + (range + PRINT_MARGINS) < Globals.GAME_FIELD_BOTTOM_RIGHT.X)
                xEnd = FOV_X + (range + PRINT_MARGINS);
            else
                xEnd = Globals.GAME_FIELD_BOTTOM_RIGHT.X;

            if (FOV_Y - (range + PRINT_MARGINS) >= 0)
                yStart = FOV_Y - (range + PRINT_MARGINS);
            else
                yStart = 0;

            if (FOV_Y + (range + PRINT_MARGINS) < Globals.GAME_FIELD_BOTTOM_RIGHT.Y)
                yEnd = FOV_Y + (range + PRINT_MARGINS);
            else
                yEnd = Globals.GAME_FIELD_BOTTOM_RIGHT.Y;

            //the actual print loop
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (map[x, y].IsVisible == true)
                    {
                        ConsoleTools.WriteOnPosition(map[x, y].Terrain);
                        if (map[x, y].IngameObject != null)
                        {
                            ConsoleTools.WriteOnPosition(map[x, y].IngameObject);
                        }
                        if (map[x, y].Item != null)
                        {
                            ConsoleTools.WriteOnPosition(itemCharacters[map[x, y].Item.ItemType.ItemCode.BaseTypeInt], x, y);
                        }
                        if (map[x, y].Unit != null)
                        {
                            if (map[x, y].Unit.VisualChar != '@')
                            {
                                GameEngine.GameField[x, y].Unit = new Unit(map[x,y].Unit);
                                if (x >= xStart && x <= xEnd && y >= yStart && y <= yEnd)
                                {
                                    if (GameEngine.GameField[x, y].IsVisible)
                                        ConsoleTools.WriteOnPosition(map[x, y].Unit);
                                }
                            }
                            else
                            {
                                GameEngine.GameField[x, y].Unit = new Unit(map[x, y].Unit);
                                fieldOfView.ComputeFov(x, y, range, true, method, shape);
                                ConsoleTools.WriteOnPosition(map[x, y].Unit);
                            }
                        }
                    }
                    else
                    {
                        ConsoleTools.WriteOnPosition(' ', x, y);
                    }
                }
            }
        }

        public void PrintUnit(Unit unit)
        {
            GameEngine.GameField[unit.X, unit.Y].Unit = new Unit(unit);
            if (unit.VisualChar == '@')
            {
                ConsoleTools.WriteOnPosition(unit);
                this.PrintFOVMap(unit.X, unit.Y);
            }
            else if (unit.X >= xStart && unit.X <= xEnd && unit.Y >= yStart && unit.Y <= yEnd)
            {
                if (GameEngine.GameField[unit.X, unit.Y].IsVisible)
                    ConsoleTools.WriteOnPosition(unit);
            }
        }
                
        public void ClearGameObject(Unit unit)
        {
            if (unit.X >= xStart && unit.X <= xEnd && unit.Y >= yStart && unit.Y <= yEnd)
            {
                ConsoleTools.WriteOnPosition(GameEngine.GameField[unit.X, unit.Y].Terrain, unit.X, unit.Y);
            }
            else ConsoleTools.WriteOnPosition(' ', unit.X, unit.Y);
            GameEngine.GameField[unit.X, unit.Y].Unit = null;
        }
    }
}