using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static bool unSaved = false;
        public static bool Unsaved
        {
            get { return unSaved; }
            set {
                unSaved = value;

                if (unSaved)
                {
                    if (MainWindow.Instance.Title[MainWindow.Instance.Title.Length - 1] != '*')
                        MainWindow.Instance.Title = MainWindow.Instance.Title + " *";
                }
                else
                {
                    if (MainWindow.Instance.Title[MainWindow.Instance.Title.Length - 1] == '*')
                        MainWindow.Instance.Title = MainWindow.Instance.Title.Substring(0, MainWindow.Instance.Title.Length - 2);
                }
            }
        }

        public static string OpenFilePath;

        public static MouseEventArgs MouseEventArgs;
        public static int TileSize;
        public static List<Tile> Tiles;
        public static ActionTypes ActionType;

        private static int tileLayer;
        public static int TileLayer
        {
            get { return tileLayer; }
            set {
                Global.Unsaved = true;
                tileLayer = value;

                if (MainWindow.Instance.TileLayerComboBox.SelectedIndex != tileLayer)
                    MainWindow.Instance.TileLayerComboBox.SelectedIndex = tileLayer;
            }
        }

        public static List<string> Layers;

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
                Global.Unsaved = true;

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
                Global.Tiles = new List<Tile>();
                Global.StatusBarTextLeft = "Updated map size to " + mapSize.X.ToString() + " x "+ mapSize.Y.ToString();
            }
        }

        public static int WrapValue(int x, int x_min, int x_max)
        {
            if (x == 0 && x_min == 0)
                return 0;

            return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
        }

        public static bool AddLayer(string alias, int? insertIndex = null)
        {
            if (alias.Length == 0)
            {
                MessageBox.Show("Layer must have a name");
                return false;
            }

            if (Global.Layers.Contains(alias))
            {
                MessageBox.Show("A layer with this name already exists");
                return false;
            }

            if (insertIndex == null)
                insertIndex = MainWindow.Instance.TileLayerComboBox.Items.Count-1;

            Global.Layers.Insert((int)insertIndex+1, alias);
            MainWindow.Instance.TileLayerComboBox.Items.Insert((int)insertIndex+1, alias);
            Global.TileLayer = (int)insertIndex+1;

            return true;
        }

        public static void RemoveLayer(string alias)
        {
            Global.RemoveLayer(MainWindow.Instance.TileLayerComboBox.Items.IndexOf(alias));
        }

        public static void RemoveLayer(int index)
        {
            if (Global.Layers.Count == 1)
            {
                MessageBox.Show("There must be at least one layer");
                return;
            }

            Global.Layers.RemoveAt(index);
            MainWindow.Instance.TileLayerComboBox.Items.RemoveAt(index);
            Global.TileLayer = (index >= 1) ? index-1 : 0;
        }

        public static void ResetLayers()
        {
            Global.Layers = new List<string>();
            MainWindow.Instance.TileLayerComboBox.Items.Clear();
        }

        public static Tile GetTile(int x, int y, int z)
        {
            return Global.Tiles.Find(t => t.Location.X == x && t.Location.Y == y && t.Layer == z);
        }

        public static Tile GetTile(Point location, int z)
        {
            return Global.GetTile(location.X, location.Y, z);
        }

        // RunOnEventLoop
        public static List<string> ConditionTracker = new List<string>();
        public static void RunOnEventLoop(string key, bool condition, Action action)
        {
            if (condition && !ConditionTracker.Contains(key))
            {
                ConditionTracker.Add(key);
                action();
            }
            
            if(!condition && ConditionTracker.Contains(key))
                ConditionTracker.Remove(key);
        }

        // KeyCombo
        public static bool KeyComboDown(params Key[] keys)
        {
            foreach (Key key in keys)
                if (!Keyboard.IsKeyDown(key))
                    return false;

            return true;
        }

        // Commands
        private static bool DialogWindowOpen = false;
        public static void Command_New()
        {
            if (DialogWindowOpen)
                return;

            DialogWindowOpen = true;
            LevelSettings ls = new LevelSettings();
            ls.ShowDialog();
            DialogWindowOpen = false;
        }
        
        public static void Command_Open()
        {
            if (DialogWindowOpen)
                return;

            DialogWindowOpen = true;
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.ShowDialog(MainWindow.Instance);

            if (sfd.FileName != "")
            {
                SaverLoader sl = new SaverLoader();
                sl.Load(sfd.FileName);
                Global.OpenFilePath = sfd.FileName;
            }
            Global.Unsaved = false;
            DialogWindowOpen = false;
        }

        public static void Command_Close()
        {
            if (Global.Unsaved)
            {
                MessageBoxResult rsltMessageBox = MessageBox.Show("Would you like to save before closing?", "Do you want to save?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        Global.Command_Save();
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            MainWindow.Instance.CloseMap();
        }
        
        public static void Command_Save()
        {
            if (DialogWindowOpen)
                return;

            DialogWindowOpen = true;
            if (Global.OpenFilePath != null && Global.OpenFilePath.Length > 0)
            {
                SaverLoader sl = new SaverLoader();
                sl.SaveAs(Global.OpenFilePath);
                Global.Unsaved = false;
            }
            else
            {
                Command_SaveAs();
            }
            DialogWindowOpen = false;
        }

        public static void Command_SaveAs()
        {
            if (DialogWindowOpen)
                return;

            DialogWindowOpen = true;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog(MainWindow.Instance);

            if (sfd.FileName != "")
            {
                SaverLoader sl = new SaverLoader();
                sl.SaveAs(sfd.FileName);
                Global.Unsaved = false;
            }

            DialogWindowOpen = false;
        }

        public static void Command_Exit()
        {
            MainWindow.Instance.Close();
        }
    }
}
