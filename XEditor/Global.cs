using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XEditor
{
    public enum States
    {
        Null,
        Initialised,
        MapOpen,
        MapClosed
    }

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
        private static States state;
        public static States State
        {
            get { return state; }
            set {
                if (state == value)
                    return;
                
                state = value;

                if(state == States.Initialised)
                {
                    Global.StatusBarTextLeft = "Initialised";
                }
                else if (state == States.MapOpen)
                {
                    Global.StatusBarTextLeft = "New map opened";
                    MainWindow.Instance.EditorContainer.Visibility = Visibility.Visible;
                    MainWindow.Instance.Menu_LevelSettings.IsEnabled = true;
                    MainWindow.Instance.Menu_Close.IsEnabled = true;
                    MainWindow.Instance.Menu_Save.IsEnabled = true;
                    MainWindow.Instance.Menu_SaveAs.IsEnabled = true;
                }
                else
                {
                    Global.StatusBarTextLeft = "Map closed";
                    MainWindow.Instance.EditorContainer.Visibility = Visibility.Hidden;
                    MainWindow.Instance.Menu_LevelSettings.IsEnabled = false;
                    MainWindow.Instance.Menu_Close.IsEnabled = false;
                    MainWindow.Instance.Menu_Save.IsEnabled = false;
                    MainWindow.Instance.Menu_SaveAs.IsEnabled = false;
                }
            }
        }

        public static MouseEventArgs MouseEventArgs;
        public static int TileSize;
        public static Tile[,] Tiles;
        public static ActionTypes ActionType;

        public static SelectorModes SelectorMode;
        public static Point SelectorIndex;
        public static Point SelectorHoldIndex;
        public static Rect SelectorSelection;

        public static SelectorModes TileSelectorMode;
        public static Point TileSelectorIndex;
        public static Point TileSelectorHoldIndex;
        public static Rect TileSelectorSelection;

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

        public static BitmapImage Bitmap { get; private set; }
        private static string texturePath;
        public static string TexturePath
        {
            get { return texturePath; }
            set {
                texturePath = value;

                Bitmap = new BitmapImage(new Uri(texturePath));
                MainWindow.Instance.TilesetGrid.Background = new ImageBrush(Bitmap);
                MainWindow.Instance.TilesetGrid.Width = Bitmap.Width;
                MainWindow.Instance.TilesetGrid.Height = Bitmap.Height;
                MainWindow.Instance.TilesetSelectArea(new Rect(0, 0, 1, 1));
            }
        }
        public static Tile[,] SelectedTiles;

        private static Point mapSize;
        public static Point MapSize
        {
            get { return mapSize; }
            set {
                mapSize = value;
                MainWindow.Instance.EditorGrid.Width = mapSize.X * Global.TileSize;
                MainWindow.Instance.EditorGrid.Height = mapSize.Y * Global.TileSize;
                Global.Tiles = new Tile[(int)mapSize.X, (int)mapSize.Y];
                Global.StatusBarTextLeft = "Updated map size to " + mapSize.X.ToString() + " x "+ mapSize.Y.ToString();
            }
        }

        public static int WrapValue(int x, int x_min, int x_max)
        {
            if (x == 0 && x_min == 0)
                return 0;

            return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
        }
    }
}
