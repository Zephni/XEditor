using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace XEditor
{
    public enum SelectorModes
    {
        Normal,
        SelectingArea
    }

    public enum ActionTypes
    {
        AddingTiles,
        RemovingTiles
    }

    public static class Global
    {
        public static MouseEventArgs MouseEventArgs;

        public static int TileSize;

        public static List<Tile> Tiles;

        public static ActionTypes ActionType;

        public static SelectorModes SelectorMode;
        public static Point SelectorIndex;
        public static Point SelectorHoldIndex;
        public static Rect SelectorSelection;
    }
}
