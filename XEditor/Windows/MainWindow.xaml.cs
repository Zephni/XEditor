using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XEditor
{
    public partial class MainWindow : Window
    {
        // Properties
        public static MainWindow Instance;
        public bool MouseBusy = false;
        public List<Entity> highlightingEntities;
        public bool DraggingEntity;

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
            Global.Entities = new List<Entity>();
            new Updater().Start(17, Update);
        }

        // Updates
        public void Update()
        {
            MouseUpdates(Global.MouseEventArgs);

            Global.RunOnEventLoop("Ctrl+N", Global.KeyComboDown(Key.LeftCtrl, Key.N), () => {
                Global.Command_New();
            });

            Global.RunOnEventLoop("Ctrl+S", Global.KeyComboDown(Key.LeftCtrl, Key.S) && Global.State == States.MapOpen, () => {
                Global.Command_Save();
            });

            Global.RunOnEventLoop("Ctrl+O", Global.KeyComboDown(Key.LeftCtrl, Key.O), () => {
                Global.Command_Open();
            });
        }

        public void MouseUpdates(MouseEventArgs e)
        {
            if (!MouseBusy && Global.State == States.MapOpen)
            {
                if(Global.ToolType == ToolTypes.TilePlacer)
                {
                    TilePlacer_EditorGrid_MouseUpdates(e);
                    TilePlacer_Tileset_MouseUpdates(e);
                }
                else if (Global.ToolType == ToolTypes.TileSelector)
                {

                }
                else if (Global.ToolType == ToolTypes.Entities)
                {
                    Entities_EditorGrid_MouseUpdates(e);
                }
            }
        }

        // TilePlacer updates
        private void TilePlacer_EditorGrid_MouseUpdates(MouseEventArgs e)
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
                                tile.Layer = Global.TileLayer;

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
        private void TilePlacer_Tileset_MouseUpdates(MouseEventArgs e)
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

        // TileSelector updates

        // Entities updates
        private void Entities_EditorGrid_MouseUpdates(MouseEventArgs e)
        {
            if(EditorGrid.IsMouseOver)
            {
                if (!DraggingEntity)
                {
                    Global.SelectorIndex = new Point((int)e.GetPosition(EditorGrid).X / Global.TileSize, (int)e.GetPosition(EditorGrid).Y / Global.TileSize);
                    // Need to also select when width/height is covered, possibly..
                    highlightingEntities = Global.Entities.FindAll(t => t.Position.X == Global.SelectorIndex.X && t.Position.Y == Global.SelectorIndex.Y);
                    Global.StatusBarTextRight = (highlightingEntities.Count > 0) ? highlightingEntities[highlightingEntities.Count - 1].Name : "";
                }

                if(highlightingEntities.Count > 0 && Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    DraggingEntity = true;
                }

                if(DraggingEntity)
                {
                    // Need to add an offset here
                    Global.SelectorIndex = new Point((int)e.GetPosition(EditorGrid).X / Global.TileSize, (int)e.GetPosition(EditorGrid).Y / Global.TileSize);

                    highlightingEntities[highlightingEntities.Count-1].Position = new Point(Global.SelectorIndex.X, Global.SelectorIndex.Y);

                    if (Mouse.LeftButton == MouseButtonState.Released)
                    {
                        Entity newTopMost = highlightingEntities[highlightingEntities.Count - 1];
                        Global.Entities.Remove(newTopMost);
                        Global.Entities.Add(newTopMost);
                        DraggingEntity = false;
                    }
                }

                if (Mouse.RightButton == MouseButtonState.Pressed)
                {
                    MouseBusy = true;
                    ContextMenu entityContextMenu = new ContextMenu();
                    entityContextMenu.PlacementTarget = EditorGrid;

                    List<MenuItem> menuItems = new List<MenuItem>();
                    
                    MenuItem addEntityOption = new MenuItem();
                    addEntityOption.Header = "Add new entity";
                    addEntityOption.Click += NewEntityOption_Click;
                    entityContextMenu.Items.Add(addEntityOption);

                    foreach(Entity entity in highlightingEntities)
                    {
                        MenuItem editEntityOption = new MenuItem();
                        editEntityOption.Header = "Edit " + entity.Name;
                        editEntityOption.CommandParameter = entity;
                        editEntityOption.Click += EditEntityOption_Click;
                        entityContextMenu.Items.Add(editEntityOption);
                    }

                    entityContextMenu.Closed += Entity_ContextMenu_Closed;
                    entityContextMenu.IsOpen = true;
                }
            }
        }

        private void NewEntityOption_Click(object sender, RoutedEventArgs e)
        {
            EntitySettings es = new EntitySettings
            {
                ApplyButtonText = "Create entity",
                _EntityName = "Entity",
                _PosX = Global.SelectorIndex.X.ToString(),
                _PosY = Global.SelectorIndex.Y.ToString(),
                _SizeX = "1",
                _SizeY = "1"
            };

            es.ShowDialog();
        }

        private void EditEntityOption_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            

            EntitySettings es = new EntitySettings
            {
                ApplyButtonText = "Apply changes",
                EditingEntity = button.CommandParameter as Entity
            };

            es.ShowDialog();
        }

        private void Entity_ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            MouseBusy = false;
        }

        // Methods
        public void NewMap(Point mapSize, string texturePath, List<string> layers)
        {
            Global.Unsaved = true;
            Global.ResetLayers();

            foreach (string layer in layers)
                Global.AddLayer(layer);
            Global.TileLayer = 1;
            
            Global.TexturePath = texturePath;
            Global.MapSize = mapSize;
            Global.State = States.MapOpen;
            Global.ToolType = ToolTypes.TilePlacer;
        }

        public void CloseMap()
        {
            RemoveAllTiles();
            Global.ResetLayers();
            Global.State = States.MapClosed;
            Global.Unsaved = false;
            Global.ToolType = ToolTypes.Null;
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
            Global.ToolType = ToolTypes.TilePlacer;
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
            Global.Unsaved = true;

            if (Global.GetTile(tile.Location.X, tile.Location.Y, tile.Layer) != null)
                RemoveTile(tile.Location.X, tile.Location.Y, tile.Layer);

            tile.Layer = tile.Layer;
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

        public bool RemoveTile(Tile tile)
        {
            Global.Unsaved = true;
            if (tile == null)
                return false;

            EditorGrid.Children.RemoveAt(Global.Tiles.IndexOf(tile) + 1);
            EditorGrid.Children.Remove(tile.Rectangle);
            Global.Tiles.Remove(tile);
            return true;
        }

        public bool RemoveTile(int x, int y, int layer)
        {
            Tile tile = Global.GetTile(x, y, layer);
            return RemoveTile(tile);
        }

        public void RemoveAllTiles()
        {
            if (Global.Tiles == null)
                return;

            for(int i = 0; i < Global.Tiles.Count; i++)
                RemoveTile(Global.Tiles[i]);

            EditorGrid.Children.RemoveRange(1, EditorGrid.Children.Count - 1);

            Global.Tiles = new List<Tile>();
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

        private void File_New_Click(object sender, RoutedEventArgs e)
        {
            Global.Command_New();
        }

        private void File_Close_Click(object sender, RoutedEventArgs e)
        {
            Global.Command_Close();
        }

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            Global.Command_Open();
        }

        private void File_Save_Click(object sender, RoutedEventArgs e)
        {
            Global.Command_Save();
        }

        private void File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Global.Command_SaveAs();
        }

        private void File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Global.Command_Exit();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Global.Unsaved)
            {
                MessageBoxResult rsltMessageBox = MessageBox.Show("Would you like to save before quiting?", "Do you want to save?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        Global.Command_Save();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void ToolSwitcher_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            ToolTypes toolType = ToolTypes.Null;

            if(radioButton != null)
            {
                if (radioButton.Content.ToString() == "Tile placer")
                    toolType = ToolTypes.TilePlacer;
                else if (radioButton.Content.ToString() == "Tile selector")
                    toolType = ToolTypes.TileSelector;
                else if (radioButton.Content.ToString() == "Entities")
                    toolType = ToolTypes.Entities;
            }
            
            Global.ToolType = toolType;
        }
    }
}