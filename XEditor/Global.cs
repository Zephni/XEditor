using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

    public enum ToolTypes
    {
        Null,
        TilePlacer,
        TileSelector,
        Entities
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

    public enum YesNoCancel
    {
        Cancel,
        Yes,
        No
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

                if (state == States.MapOpen)
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

        public static IniFile Preferences = new IniFile(System.AppDomain.CurrentDomain.BaseDirectory + "/XEditorPrefences.ini");

        public static List<Tile> CopiedTiles = new List<Tile>();
        public static Point2D CopiedRectOffset;

        private static ToolTypes toolType;
        public static ToolTypes ToolType
        {
            get { return toolType; }
            set {
                if (toolType == value)
                    return;

                toolType = value;

                if (toolType == ToolTypes.TilePlacer)
                {
                    MainWindow.Instance.RadioButton_TilePlacer.IsChecked = true;
                    MainWindow.Instance.EditorGrid.Children.Remove(MainWindow.Instance.TileSelector_RectangleSelected);
                    MainWindow.Instance.TileSelector_RectangleSelected = null;
                    Global.GetSelectedEntities(entity => entity.Selected = false);
                    Global.StatusBarTextLeft = "Switched to tile placer mode";
                }  
                else if (toolType == ToolTypes.TileSelector)
                {
                    MainWindow.Instance.RadioButton_TileSelector.IsChecked = true;
                    Global.GetSelectedEntities(entity => entity.Selected = false);
                    Global.StatusBarTextLeft = "Switched to tile selector mode";
                }
                else if(toolType == ToolTypes.Entities)
                {
                    MainWindow.Instance.RadioButton_Entities.IsChecked = true;
                    MainWindow.Instance.EditorGrid.Children.Remove(MainWindow.Instance.TileSelector_RectangleSelected);
                    MainWindow.Instance.TileSelector_RectangleSelected = null;
                    Global.StatusBarTextLeft = "Switched to entity mode";
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

        public static List<Entity> Entities;

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
                {
                    MainWindow.Instance.TileLayerComboBox.SelectedIndex = tileLayer;
                    Global.StatusBarTextLeft = "Switched to layer "+ MainWindow.Instance.TileLayerComboBox.SelectedValue;
                }
            }
        }

        public static List<string> Layers;

        public static SelectorModes SelectorMode;
        public static Point2D SelectorIndex;
        public static Point2D SelectorHoldIndex;
        public static Rect SelectorSelection;

        public static SelectorModes TileSelectorMode;
        public static Point2D TileSelectorIndex;
        public static Point2D TileSelectorHoldIndex;
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

        private static Point2D mapSize;
        public static Point2D MapSize
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

            // Note sure why have to run this more than once, but for now it works
            for(int Temp = 0; Temp < 10; Temp++)
                for (int I = 0; I < Global.Tiles.Count; I++)
                {
                    if (Global.Tiles[I].Layer == index)
                        MainWindow.Instance.RemoveTile(Global.Tiles[I]);
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

        public static Tile GetTile(Point2D location, int z)
        {
            return Global.GetTile(location.X, location.Y, z);
        }

        public static List<Entity> GetSelectedEntities(Action<Entity> entityAction = null)
        {
            List<Entity> entities = Global.Entities.FindAll(e => e.Selected == true);
            if (entityAction != null)
                for (int i = 0; i < entities.Count; i++)
                    entityAction(entities[i]);
            return entities;
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

        public static YesNoCancel ImageChecker(string tilesetPath, out string outPath)
        {
            outPath = tilesetPath;
            bool invalidImage = true;

            string invalidMsg = "";

            if (!File.Exists(tilesetPath))
                invalidMsg = "Could not find tileset ("+tilesetPath+"), browse for file?";

            ImageFormat.ImageFormats[] validifs = new ImageFormat.ImageFormats[] {
                ImageFormat.ImageFormats.bmp,
                ImageFormat.ImageFormats.png,
                ImageFormat.ImageFormats.jpeg
            };

            if (invalidMsg == "" && !validifs.Contains(ImageFormat.GetImageFormat(File.ReadAllBytes(tilesetPath))))
                invalidMsg = "Image must be png, bmp or jpg format, browse for file?";
            
            if (invalidMsg == "")
            {
                invalidImage = false;
            }
            else
            {
                MessageBoxResult rsltMessageBox = MessageBox.Show(invalidMsg, "Tileset error", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (rsltMessageBox == MessageBoxResult.Yes)
                {
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.FileName = tilesetPath;
                    ofd.RestoreDirectory = true;
                    if (ofd.ShowDialog(MainWindow.Instance) == true)
                    {
                        outPath = ofd.FileName;
                        return ImageChecker(outPath, out outPath);
                    }
                }
                else
                {
                    return YesNoCancel.Cancel;
                }
            }              

            return (!invalidImage) ? YesNoCancel.Yes : YesNoCancel.No;
        }

        // Commands
        private static bool DialogWindowOpen = false;

        public static void Command_CopyTiles(Rect rect, int? layer)
        {
            CopiedRectOffset = new Point2D(Convert.ToInt16(rect.X), Convert.ToInt16(rect.Y));

            CopiedTiles = Global.Tiles.FindAll(t =>
                t.Location.X >= rect.X
                && t.Location.Y >= rect.Y
                && t.Location.X < rect.X + rect.Width
                && t.Location.Y < rect.Y + rect.Height
                && t.Layer == ((layer != null) ? layer : t.Layer)
            );

            StatusBarTextLeft = "Copied " + CopiedTiles.Count + " tiles)";
        }

        public static void Command_CutTiles(Rect rect, int? layer)
        {
            Command_CopyTiles(rect, layer);
            foreach (Tile tile in CopiedTiles)
                MainWindow.Instance.RemoveTile(tile);

            StatusBarTextLeft = "Cut " + CopiedTiles.Count + " tiles)";
        }

        public static void Command_PasteTiles(Rect rect, int? layer)
        {
            if (CopiedTiles == null || CopiedTiles.Count == 0)
                return;

            foreach (Tile tile in CopiedTiles)
            {
                Tile newTile = tile.DeepCopy();

                newTile.Location = new Point2D(
                    Convert.ToInt16((tile.Location.X - CopiedRectOffset.X) + rect.X),
                    Convert.ToInt16((tile.Location.Y - CopiedRectOffset.Y) + rect.Y)
                );

                if (layer != null)
                    newTile.Layer = (int)layer;

                MainWindow.Instance.AddTile(newTile);
            }

            StatusBarTextLeft = "Pasted " + CopiedTiles.Count + " tiles)";
        }

        public static List<Entity> CopiedEntities = new List<Entity>();
        public static void Command_CopyEntities(List<Entity> highlightedEntities)
        {
            if (highlightedEntities.Count == 0)
                return;
            Entity[] copied = new Entity[highlightedEntities.Count];
            highlightedEntities.CopyTo(copied);
            CopiedEntities = copied.ToList();

            StatusBarTextLeft = "Copied " + highlightedEntities[0].Name;
        }

        public static void Command_CutEntities(List<Entity> highlightedEntities)
        {
            if (highlightedEntities.Count == 0)
                return;
            Command_CopyEntities(highlightedEntities);

            for (int I = 0; I < highlightedEntities.Count; I++)
                highlightedEntities[I].Destroy();

            StatusBarTextLeft = "Cut " + highlightedEntities[0].Name;
        }

        public static void Command_PasteEntities(Point2D newPosition)
        {
            foreach(var item in CopiedEntities)
            {
                Entity entity = new Entity
                {
                    Name = item.Name,
                    Position = newPosition,
                    Size = new Point2D(Convert.ToInt16(item.Size.X), Convert.ToInt16(item.Size.Y)),
                    CustomData = item.CustomData
                };

                Global.Entities.Add(entity);
            }

            StatusBarTextLeft = "Pasted " + CopiedEntities[0].Name;
        }

        public static void Command_RemoveTiles(Rect rect, int? layer)
        {
            List<Tile> tiles = Global.Tiles.FindAll(t =>
                t.Location.X >= rect.X
                && t.Location.Y >= rect.Y
                && t.Location.X < rect.X + rect.Width
                && t.Location.Y < rect.Y + rect.Height
                && t.Layer == ((layer != null) ? layer : t.Layer)
            );

            foreach (Tile tile in tiles)
                MainWindow.Instance.RemoveTile(tile);

            StatusBarTextLeft = "Removed " + CopiedTiles.Count + " tiles)";
        }

        public static void Command_New()
        {
            if (DialogWindowOpen)
                return;

            DialogWindowOpen = true;
            LevelSettings ls = new LevelSettings(true);
            ls.ShowDialog();
            DialogWindowOpen = false;
        }
        
        public static void Command_Open(string filePath = "")
        {
            if (DialogWindowOpen)
                return;

            if(filePath.Length == 0)
            {
                DialogWindowOpen = true;
                OpenFileDialog sfd = new OpenFileDialog();
                sfd.ShowDialog(MainWindow.Instance);

                if (sfd.FileName != "")
                {
                    filePath = sfd.FileName;
                    RecentFilesController.AddFile(sfd.FileName);
                }
            }

            if (filePath != "")
            {
                SaverLoader sl = new SaverLoader();
                sl.Load(filePath);
                Global.OpenFilePath = filePath;
                RecentFilesController.AddFile(filePath);
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
                DialogWindowOpen = false;
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
            sfd.AddExtension = true;
            sfd.Filter = "Level Files (*.lvl)|*.lvl";
            sfd.DefaultExt = "lvl";
            sfd.ShowDialog(MainWindow.Instance);

            if (sfd.FileName != "")
            {
                SaverLoader sl = new SaverLoader();
                sl.SaveAs(sfd.FileName);
                Global.Unsaved = false;
                Global.OpenFilePath = sfd.FileName;

                RecentFilesController.AddFile(sfd.FileName);
            }

            DialogWindowOpen = false;
        }

        public static void Command_Exit()
        {
            MainWindow.Instance.Close();
        }
    }
}
