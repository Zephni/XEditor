using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XEditor
{
    public partial class MainWindow : Window
    {
        // Properties
        public static MainWindow Instance;
        
        // Constructor
        public MainWindow()
        {
            Instance = this;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Selector.Visibility = Visibility.Hidden;
            Global.TileSize = 16;
            Global.State = States.Initialised;
            Global.Layers = new List<string>();
            new Updater().Start(17, Update);
        }

        // Updates
        public void Update()
        {
            MouseUpdates(Global.MouseEventArgs);
            Global.StatusBarTextRight = string.Join(", ", Global.Layers);
        }

        public void MouseUpdates(MouseEventArgs e)
        {
            if (Global.State == States.MapOpen)
            {
                EditorGridMouseUpdates(e);
                TilesetMouseUpdates(e);
            }
        }

        public void EditorGridMouseUpdates(MouseEventArgs e)
        {
            Rectangle ThisSelector = Selector;

            if (EditorGrid.IsMouseOver)
            {
                ThisSelector.Stroke = new SolidColorBrush(Colors.Black);

                ThisSelector.Visibility = Visibility.Visible;
                Global.SelectorIndex = new Point((int)e.GetPosition(EditorGrid).X / Global.TileSize, (int)e.GetPosition(EditorGrid).Y / Global.TileSize);

                if (Global.SelectorMode == SelectorModes.Normal)
                {
                    Global.StatusBarTextRight = Global.SelectorIndex.ToString();

                    if (Global.GetTile(Global.SelectorIndex, Global.TileLayer) != null)
                        Global.StatusBarTextRight += " (tile "+ Global.GetTile(Global.SelectorIndex, Global.TileLayer).TilesetLocation.ToString() + ")";

                    ThisSelector.Margin = new Thickness(Global.SelectorIndex.X * Global.TileSize, Global.SelectorIndex.Y * Global.TileSize, 0, 0);
                    ThisSelector.Width = Global.TileSize + 1;
                    ThisSelector.Height = Global.TileSize + 1;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Global.SelectorHoldIndex = new Point(Global.SelectorIndex.X, Global.SelectorIndex.Y);
                        Global.SelectorMode = SelectorModes.SelectingArea;
                        Global.ActionType = ActionTypes.AddingTiles;
                    }

                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        Global.SelectorHoldIndex = new Point(Global.SelectorIndex.X, Global.SelectorIndex.Y);
                        Global.SelectorMode = SelectorModes.SelectingArea;
                        Global.ActionType = ActionTypes.RemovingTiles;
                    }
                }
                else if (Global.SelectorMode == SelectorModes.SelectingArea)
                {
                    if (Global.ActionType == ActionTypes.AddingTiles)
                        ThisSelector.Stroke = new SolidColorBrush(Colors.Green);
                    else if (Global.ActionType == ActionTypes.RemovingTiles)
                        ThisSelector.Stroke = new SolidColorBrush(Colors.Red);

                    int LeftMost = (int)((Global.SelectorIndex.X >= Global.SelectorHoldIndex.X) ? Global.SelectorHoldIndex.X : Global.SelectorIndex.X);
                    int TopMost = (int)((Global.SelectorIndex.Y >= Global.SelectorHoldIndex.Y) ? Global.SelectorHoldIndex.Y : Global.SelectorIndex.Y);
                    int Width = (int)Math.Abs(Global.SelectorHoldIndex.X - Global.SelectorIndex.X);
                    int Height = (int)Math.Abs(Global.SelectorHoldIndex.Y - Global.SelectorIndex.Y);

                    ThisSelector.Margin = new Thickness(LeftMost * Global.TileSize, TopMost * Global.TileSize, 0, 0);
                    ThisSelector.Width = Width * Global.TileSize + (Global.TileSize + 1);
                    ThisSelector.Height = Height * Global.TileSize + (Global.TileSize + 1);

                    if (Global.ActionType == ActionTypes.AddingTiles)
                        Global.StatusBarTextRight = "Drawing to area " + LeftMost + ", " + TopMost + " (size " + (Width + 1) + ", " + (Height + 1) + ")";
                    else if (Global.ActionType == ActionTypes.RemovingTiles)
                        Global.StatusBarTextRight = "Deleting area " + LeftMost + ", " + TopMost + " (size " + (Width + 1) + ", " + (Height + 1) + ")";

                    if (Global.ActionType == ActionTypes.AddingTiles && e.LeftButton == MouseButtonState.Released)
                    {
                        Global.SelectorMode = SelectorModes.Normal;
                        int tileCount = 0;

                        if (Global.SelectedTiles == null)
                            return;

                        // Place tile/s
                        for (int x = LeftMost; x <= LeftMost + Width; x++)
                        {
                            for (int y = TopMost; y <= TopMost + Height; y++)
                            {
                                tileCount++;
                                int X = Global.WrapValue(x - LeftMost, 0, Global.SelectedTiles.GetLength(0));
                                int Y = Global.WrapValue(y - TopMost, 0, Global.SelectedTiles.GetLength(1));
                                Tile tile = Global.SelectedTiles[X, Y].DeepCopy();
                                tile.Location = new Point(x, y);

                                // Remove old if exists
                                RemoveTile(x, y, Global.TileLayer);

                                // Add new
                                AddTile(tile);
                            }
                        }

                        Global.StatusBarTextLeft = "Added " + tileCount + " tiles";
                    }

                    if (Global.ActionType == ActionTypes.RemovingTiles && e.RightButton == MouseButtonState.Released)
                    {
                        Global.SelectorMode = SelectorModes.Normal;
                        int tileCount = 0;

                        for (int X = LeftMost; X <= LeftMost + Width; X++)
                        {
                            for (int Y = TopMost; Y <= TopMost + Height; Y++)
                            {
                                if (RemoveTile(X, Y, Global.TileLayer))
                                    tileCount++;
                            }
                        }

                        Global.StatusBarTextLeft = "Removed " + tileCount + " tiles";
                    }
                }
            }
            else
            {
                ThisSelector.Visibility = Visibility.Hidden;
            }
        }

        public void TilesetMouseUpdates(MouseEventArgs e)
        {
            Rectangle ThisSelector = TilesetSelector;
            ThisSelector.Stroke = new SolidColorBrush(Colors.White);

            if (TilesetGrid.IsMouseOver)
            {
                ThisSelector.Visibility = Visibility.Visible;
                Global.TileSelectorIndex = new Point((int)(e.GetPosition(TilesetGrid).X / Global.TileSize), (int)(e.GetPosition(TilesetGrid).Y / Global.TileSize));

                if (Global.TileSelectorMode == SelectorModes.Normal)
                {
                    Global.StatusBarTextRight = Global.SelectorIndex.ToString();

                    ThisSelector.Margin = new Thickness(Global.TileSelectorIndex.X * Global.TileSize, Global.TileSelectorIndex.Y * Global.TileSize, 0, 0);
                    ThisSelector.Width = Global.TileSize + 1;
                    ThisSelector.Height = Global.TileSize + 1;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Global.TileSelectorHoldIndex = new Point(Global.TileSelectorIndex.X, Global.TileSelectorIndex.Y);
                        Global.TileSelectorMode = SelectorModes.SelectingArea;
                    }
                }
                else if (Global.TileSelectorMode == SelectorModes.SelectingArea)
                {
                    int LeftMost = (int)((Global.TileSelectorIndex.X >= Global.TileSelectorHoldIndex.X) ? Global.TileSelectorHoldIndex.X : Global.TileSelectorIndex.X);
                    int TopMost = (int)((Global.TileSelectorIndex.Y >= Global.TileSelectorHoldIndex.Y) ? Global.TileSelectorHoldIndex.Y : Global.TileSelectorIndex.Y);
                    int Width = (int)Math.Abs(Global.TileSelectorHoldIndex.X - Global.TileSelectorIndex.X) + 1;
                    int Height = (int)Math.Abs(Global.TileSelectorHoldIndex.Y - Global.TileSelectorIndex.Y) + 1;

                    ThisSelector.Margin = new Thickness(LeftMost * Global.TileSize, TopMost * Global.TileSize, 0, 0);
                    ThisSelector.Width = Width * Global.TileSize;
                    ThisSelector.Height = Height * Global.TileSize;

                    if (e.LeftButton == MouseButtonState.Released)
                    {
                        Global.TileSelectorMode = SelectorModes.Normal;
                        TilesetSelectArea(new Rect(LeftMost, TopMost, Width, Height));
                        Global.StatusBarTextLeft = "Selected tileset area";
                    }
                }
            }
            else
            {
                ThisSelector.Visibility = Visibility.Hidden;
            }
        }

        // Methods
        public void NewMap(Point mapSize, string texturePath, List<string> layers)
        {
            for (int i = 0; i < Global.Layers.Count; i++)
                Global.RemoveLayer(i);

            foreach(string layer in layers)
                Global.AddLayer(layer);
            Global.TileLayer = 1;
            
            Global.TexturePath = texturePath;
            Global.MapSize = mapSize;
            Global.State = States.MapOpen;
        }

        public void CloseMap()
        {
            for(int i = 0; i < Global.Tiles.Count; i++)
                RemoveTile(Global.Tiles[i].Location.X, Global.Tiles[i].Location.Y, Global.Tiles[i].Layer);                       

            Global.State = States.MapClosed;
        }

        public void OpenMap(Point mapSize, string texturePath, List<string> layers, List<Tile> tiles)
        {
            CloseMap();
            NewMap(mapSize, texturePath, layers);

            foreach (Tile tile in tiles)
            {
                tile.Location = tile.Location;
                tile.Layer = tile.Layer;
                tile.TilesetLocation = tile.TilesetLocation;
            }

            AddTiles(tiles);
        }

        public void TilesetSelectArea(Rect rect)
        {
            TilesetSelectedArea.Visibility = Visibility.Visible;
            TilesetSelectedArea.Margin = new Thickness(rect.Left * Global.TileSize, rect.Top * Global.TileSize, 0, 0);
            TilesetSelectedArea.Width = rect.Width * Global.TileSize;
            TilesetSelectedArea.Height = rect.Height * Global.TileSize;

            // Logic here
            Global.SelectedTiles = new Tile[(int)rect.Width, (int)rect.Height];

            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    Global.SelectedTiles[x, y] = CreateTileObject((int)rect.X + x, (int)rect.Y + y);
                }
            }
        }

        public Tile CreateTileObject(int tilesetPosX, int tilesetPosY)
        {
            Tile tile = new Tile {                
                TilesetLocation = new Point(tilesetPosX, tilesetPosY)
            };

            return tile;
        }

        public void AddTile(Tile tile)
        {
            if (Global.GetTile(tile.Location.X, tile.Location.Y, tile.Layer) != null)
                RemoveTile(tile.Location.X, tile.Location.Y, tile.Layer);

            tile.Layer = Global.TileLayer;
            EditorGrid.Children.Add(tile.Rectangle);
            Global.Tiles.Add(tile);
        }

        public void AddTiles(List<Tile> tiles)
        {
            foreach (Tile tile in tiles)
                AddTile(tile);
        }

        public List<Tile> GetTileList()
        {
            List<Tile> tiles = new List<Tile>();

            if(Global.Tiles != null)
            {
                for(int i = 0; i < Global.Tiles.Count; i++)
                    tiles.Add(Global.GetTile(Global.Tiles[i].Location.X, Global.Tiles[i].Location.Y, Global.Tiles[i].Layer));
            }

            return tiles;
        }

        public bool RemoveTile(int x, int y, int layer)
        {
            Tile tile = Global.GetTile(x, y, layer);

            if (tile == null)
                return false;

            EditorGrid.Children.Remove(tile.Rectangle);
            Global.Tiles.Remove(tile);
            return true;
        }

        public void RemoveAllTiles()
        {
            if (Global.Tiles == null)
                return;

            for(int i = 0; i < Global.Tiles.Count; i++)
                RemoveTile(Global.Tiles[i].Location.X, Global.Tiles[i].Location.Y, Global.Tiles[i].Layer);
        }

        // Window events
        private void Window_MouseMove(object sender, MouseEventArgs e){Global.MouseEventArgs = e; }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { Global.MouseEventArgs = e; }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {Global.MouseEventArgs = e; }

        // Menu items
        private void LevelSettings_Click(object sender, RoutedEventArgs e)
        {
            LevelSettings ls = new LevelSettings();
            ls.ShowDialog();
        }

        private void File_New_Click(object sender, RoutedEventArgs e)
        {
            LevelSettings ls = new LevelSettings();
            ls.ShowDialog();
        }

        private void File_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseMap();
        }

        private void TileLayerComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Global.TileLayer = TileLayerComboBox.SelectedIndex;
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            AddLayer addLayer = new AddLayer();
            addLayer.ShowDialog();
        }

        private void RemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            Global.RemoveLayer(MainWindow.Instance.TileLayerComboBox.SelectedIndex);
        }

        private void MoveLayerUp_Click(object sender, RoutedEventArgs e)
        {
            int current = Global.TileLayer;

            if(current == 0)
            {
                MessageBox.Show("Cannot move layer any lower");
                return;
            }

            var item = TileLayerComboBox.Items[current];

            Global.RemoveLayer(current);
            Global.AddLayer(item.ToString(), current-2);
        }

        private void MoveLayerDown_Click(object sender, RoutedEventArgs e)
        {
            int current = Global.TileLayer;

            if (current >= Global.Layers.Count-1)
            {
                MessageBox.Show("Cannot move layer any higher");
                return;
            }

            var item = TileLayerComboBox.Items[current];

            Global.RemoveLayer(current);
            Global.AddLayer(item.ToString(), current);
        }
    }
}