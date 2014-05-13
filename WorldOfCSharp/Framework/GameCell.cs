﻿using System.Collections.Generic;
using WorldOfCSharp.FieldOfView;

namespace WorldOfCSharp
{
    public class GameCell : IFovCell
    {
        private bool isVisible;

        private Terrain terrain;
        private Unit unit;
        private InGameObject ingameObject;
        private List<Item> item = new List<Item>();
        
        public bool IsTransparent
        {
            get { return this.Terrain.Flags.HasFlag(Flags.IsTransparent); }
            set { }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { this.isVisible = value; }
        }

        public Terrain Terrain
        {
            get { return this.terrain; }
            set { this.terrain = value; }
        }

        public Unit Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }

        public InGameObject IngameObject
        {
            get { return this.ingameObject; }
            set { this.ingameObject = value; }
        }

        public List<Item> ItemList
        {
            get { return this.item; }
            set { this.item = value; }
        }
    }
}
