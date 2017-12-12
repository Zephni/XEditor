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

        public static string StatusBarTextLeft
        {
            get { return MainWindow.Instance.StatusBarTextLeft.Content.ToString(); }
            set { MainWindow.Instance.StatusBarTextLeft.Content = value; }
        }
        public static string StatusBarTextRight
        {
            get { return MainWindow.Instance.StatusBarTextRight.Content.ToString(); }
            set { MainWindow.Instance.StatusBarTextRight.Content = value; }
        }

        // Non implemented
        private static string texturePath;
        public static string TexturePath
        {
            get { return texturePath; }
            set {
                texturePath = value;
            }
        }

        private static Point mapSize;
        public static Point MapSize
        {
            get { return mapSize; }
            set {
                mapSize = value;
                MainWindow.Instance.EditorGrid.Width = mapSize.X * Global.TileSize;
                MainWindow.Instance.EditorGrid.Height = mapSize.Y * Global.TileSize;
                Global.StatusBarTextLeft = "Updated map size to " + mapSize.X.ToString() + " x "+ mapSize.Y.ToString();
            }
        }
    }
}
