using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XEditor
{
    public partial class MainWindow : Window
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        // Properties
        public static MainWindow Instance;
        public bool MouseBusy = false;
        public List<Entity> highlightingEntities;
        public bool DraggingEntity;
        public Point2D DraggingOffset;
        public bool TileSelector_Dragging = false;
        public Rectangle TileSelector_Rectangle;
        public Rectangle TileSelector_RectangleSelected;
        public Rect TileSelector_SelectedRect;
        public Point2D TileSelector_DraggingOrigin;

        private float scale = 1;
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                
                if (scale < 0.5f) scale = 0.5f;
                else if (scale > 4) scale = 4;

                EditorScale.ScaleX = scale;
                EditorScale.ScaleY = scale;
            }
        }

        public bool SaveAsCompressed
        {
            get {
                if (!Global.Preferences.KeyExists("SaveAsCompressed"))
                    Global.Preferences.Write("SaveAsCompressed", "0");

                return (Global.Preferences.Read("SaveAsCompressed") != "0");
            }
            set
            {
                Menu_SaveAsCompressed.IsChecked = value;
                Global.Preferences.Write("SaveAsCompressed", (value == false) ? "0" : "1");
            }
        }

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
            Global.StatusBarTextLeft = "Initialised";

            if(!Global.Preferences.KeyExists("DefaultLayers"))
            {
                List<string> defaultLayers = new List<string>() { "Background", "Main", "Foreground" };
                Global.Preferences.Write("DefaultLayers", String.Join("|", defaultLayers));
            }

            if (!Global.Preferences.KeyExists("DefaultSelectedLayer"))
                Global.Preferences.Write("DefaultSelectedLayer", "Main");

            Menu_SaveAsCompressed.IsChecked = SaveAsCompressed;

            LoadRecentFiles();
        }

        private bool HasClearOption = false;
        public void LoadRecentFiles()
        {
            Menu_RecentFiles.IsEnabled = false;

            Menu_RecentFiles.Items.Clear();

            List<string> recentFiles = RecentFilesController.GetRecentFiles();
            recentFiles.Reverse();

            foreach (var item in recentFiles)
            {
                MenuItem newMenuItem = new MenuItem();
                newMenuItem.Header = item;
                newMenuItem.Click += LoadRecentFile;
                Menu_RecentFiles.Items.Add(newMenuItem);
            }

            if(Menu_RecentFiles.Items.Count > 0)
            {
                HasClearOption = true;
                Menu_RecentFiles.IsEnabled = true;

                MenuItem clearRecentMenuItem = new MenuItem();
                clearRecentMenuItem.Header = "Clear";
                clearRecentMenuItem.Click += ClearRecentMenuItem_Click;
                Menu_RecentFiles.Items.Add(clearRecentMenuItem);
            }

            if(Menu_RecentFiles.Items.Count == 0)
            {
                HasClearOption = false;
                Menu_RecentFiles.IsEnabled = false;
            }
        }

        private void ClearRecentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RecentFilesController.Clear();
        }

        private void LoadRecentFile(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            Global.Command_Open(item.Header.ToString());
        }

        bool resetScrollOffset = false;

        // Updates
        public void Update()
        {
            if(resetScrollOffset && EditorScroller.HorizontalOffset > 0)
            {
                EditorScroller.ScrollToHorizontalOffset(500);
                EditorScroller.ScrollToVerticalOffset(500);
                resetScrollOffset = false;
            }
            
            MouseUpdates(Global.MouseEventArgs);

            // File menu
            Global.RunOnEventLoop("Ctrl+N", Global.KeyComboDown(Key.LeftCtrl, Key.N), () => {Global.Command_New();});
            Global.RunOnEventLoop("Ctrl+S", Global.State == States.MapOpen && Global.KeyComboDown(Key.LeftCtrl, Key.S), () => {Global.Command_Save();});
            Global.RunOnEventLoop("Ctrl+O", Global.KeyComboDown(Key.LeftCtrl, Key.O), () => {Global.Command_Open();});

            // Shortcuts
            Global.RunOnEventLoop("Ctrl+C", Global.State == States.MapOpen && Global.ToolType == ToolTypes.TileSelector && TileSelector_RectangleSelected != null && Global.KeyComboDown(Key.LeftCtrl, Key.C), () => { Global.Command_CopyTiles(TileSelector_SelectedRect, Global.TileLayer); });
            Global.RunOnEventLoop("Ctrl+X", Global.State == States.MapOpen && Global.ToolType == ToolTypes.TileSelector && TileSelector_RectangleSelected != null && Global.KeyComboDown(Key.LeftCtrl, Key.X), () => { Global.Command_CutTiles(TileSelector_SelectedRect, Global.TileLayer); });
            Global.RunOnEventLoop("Ctrl+V", Global.State == States.MapOpen && Global.ToolType == ToolTypes.TileSelector && TileSelector_RectangleSelected != null && Global.KeyComboDown(Key.LeftCtrl, Key.V), () => { Global.Command_PasteTiles(TileSelector_SelectedRect, Global.TileLayer); });
            Global.RunOnEventLoop("Delete (Tiles)", Global.State == States.MapOpen && Global.ToolType == ToolTypes.TileSelector && TileSelector_RectangleSelected != null && Global.KeyComboDown(Key.Delete), () => { Global.Command_RemoveTiles(TileSelector_SelectedRect, Global.TileLayer); });
            Global.RunOnEventLoop("Delete (Entities)", Global.State == States.MapOpen && Global.ToolType == ToolTypes.Entities && Global.KeyComboDown(Key.Delete), () => { Global.GetSelectedEntities(entity => entity.Destroy()); });
        }

        public void MouseUpdates(MouseEventArgs e)
        {
            if (e == null)
                return;

            if (!MouseBusy && Global.State == States.MapOpen)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    EditorScroller.ScrollToHorizontalOffset((e.GetPosition(EditorMargin).X * Scale) - (EditorScroller.ActualWidth / 2) + 23);
                    EditorScroller.ScrollToVerticalOffset((e.GetPosition(EditorMargin).Y * Scale) - (EditorScroller.ActualHeight / 2) + 23);

                    Point temp = EditorScroller.PointToScreen(new Point(-23 + EditorScroller.RenderSize.Width/2, -23 + EditorScroller.RenderSize.Height/2));

                    // If mouse around edges
                    Point mousePos = Mouse.GetPosition(this);
                    Point mousePosScreen = PointToScreen(mousePos);
                    if (e.GetPosition(EditorMargin).X < (EditorScroller.RenderSize.Width / 2) * (1 - Scale)) temp.X = mousePosScreen.X;
                    if (e.GetPosition(EditorMargin).Y < (EditorScroller.RenderSize.Height / 2) * (1 - Scale)) temp.Y = mousePosScreen.Y;
                    if (e.GetPosition(EditorMargin).X > EditorMargin.Width - (EditorScroller.RenderSize.Width / 2) * (1 - Scale)) temp.X = mousePosScreen.X;
                    if (e.GetPosition(EditorMargin).Y > EditorMargin.Height - (EditorScroller.RenderSize.Height / 2) * (1 - Scale)) temp.Y = mousePosScreen.Y;

                    SetCursorPos((int)temp.X, (int)temp.Y);
                }

                if (Global.ToolType == ToolTypes.TilePlacer)
                {
                    TilePlacer_EditorGrid_MouseUpdates(e);
                    TilePlacer_Tileset_MouseUpdates(e);
                }
                else if (Global.ToolType == ToolTypes.TileSelector)
                {
                    TileSelector_EditorGrid_MouseUpdates(e);
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
                Global.SelectorIndex = new Point2D((int)e.GetPosition(EditorGrid).X / Global.TileSize, (int)e.GetPosition(EditorGrid).Y / Global.TileSize);

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
                        Global.SelectorHoldIndex = new Point2D(Global.SelectorIndex.X, Global.SelectorIndex.Y);
                        Global.SelectorMode = SelectorModes.SelectingArea;
                        Global.ActionType = ActionTypes.AddingTiles;
                    }

                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        Global.SelectorHoldIndex = new Point2D(Global.SelectorIndex.X, Global.SelectorIndex.Y);
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
                                tile.Location = new Point2D(x, y);
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
                Global.TileSelectorIndex = new Point2D((int)(e.GetPosition(TilesetGrid).X / Global.TileSize), (int)(e.GetPosition(TilesetGrid).Y / Global.TileSize));

                if (Global.TileSelectorMode == SelectorModes.Normal)
                {
                    Global.StatusBarTextRight = Global.SelectorIndex.ToString();

                    ThisSelector.Margin = new Thickness(Global.TileSelectorIndex.X * Global.TileSize, Global.TileSelectorIndex.Y * Global.TileSize, 0, 0);
                    ThisSelector.Width = Global.TileSize + 1;
                    ThisSelector.Height = Global.TileSize + 1;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Global.TileSelectorHoldIndex = new Point2D(Global.TileSelectorIndex.X, Global.TileSelectorIndex.Y);
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
        public void TileSelector_EditorGrid_MouseUpdates(MouseEventArgs e)
        {
            if (EditorGrid.IsMouseOver)
            {
                Point2D MouseIndex = new Point2D((int)(e.GetPosition(EditorGrid).X / Global.TileSize), (int)(e.GetPosition(EditorGrid).Y / Global.TileSize));

                if (!TileSelector_Dragging && e.LeftButton == MouseButtonState.Pressed)
                {
                    TileSelector_Dragging = true;
                    TileSelector_DraggingOrigin = new Point2D((int)(e.GetPosition(EditorGrid).X / Global.TileSize), (int)(e.GetPosition(EditorGrid).Y / Global.TileSize));
                    TileSelector_Rectangle = new Rectangle {
                        Fill = Brushes.LightSkyBlue,
                        Stroke = Brushes.CornflowerBlue,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection { 2 },
                        Opacity = 0.5d
                    };
                    TileSelector_Rectangle.SetValue(Panel.ZIndexProperty, 199);
                    EditorGrid.Children.Add(TileSelector_Rectangle);
                    EditorGrid.Children.Remove(TileSelector_RectangleSelected);
                }

                if(TileSelector_Dragging)
                {
                    TileSelector_SelectedRect = new Rect(
                        (int)((MouseIndex.X >= TileSelector_DraggingOrigin.X) ? TileSelector_DraggingOrigin.X : MouseIndex.X),
                        (int)((MouseIndex.Y >= TileSelector_DraggingOrigin.Y) ? TileSelector_DraggingOrigin.Y : MouseIndex.Y),
                        (int)(Math.Abs(TileSelector_DraggingOrigin.X - MouseIndex.X)) + 1,
                        (int)(Math.Abs(TileSelector_DraggingOrigin.Y - MouseIndex.Y)) + 1
                    );

                    TileSelector_Rectangle.Margin = new Thickness(TileSelector_SelectedRect.X * Global.TileSize, TileSelector_SelectedRect.Y * Global.TileSize, 0, 0);
                    TileSelector_Rectangle.Width = TileSelector_SelectedRect.Width * Global.TileSize;
                    TileSelector_Rectangle.Height = TileSelector_SelectedRect.Height * Global.TileSize;

                    if (TileSelector_Dragging && e.LeftButton == MouseButtonState.Released)
                    {
                        TileSelector_Dragging = false;
                        EditorGrid.Children.Remove(TileSelector_Rectangle);

                        TileSelector_RectangleSelected = new Rectangle
                        {
                            Fill = Brushes.LightSkyBlue,
                            Stroke = Brushes.CornflowerBlue,
                            StrokeThickness = 2,
                            StrokeDashArray = new DoubleCollection { 2 },
                            Opacity = 0.6d,
                            Margin = new Thickness(TileSelector_SelectedRect.X * Global.TileSize, TileSelector_SelectedRect.Y * Global.TileSize, 0, 0),
                            Width = TileSelector_SelectedRect.Width * Global.TileSize,
                            Height = TileSelector_SelectedRect.Height * Global.TileSize
                        };

                        TileSelector_RectangleSelected.SetValue(Panel.ZIndexProperty, 199);
                        EditorGrid.Children.Add(TileSelector_RectangleSelected);
                    }
                }

                if (e.RightButton == MouseButtonState.Pressed)
                {
                    EditorGrid.Children.Remove(TileSelector_RectangleSelected);
                    EditorGrid.Children.Remove(TileSelector_Rectangle);
                    TileSelector_RectangleSelected = null;
                    TileSelector_Dragging = false;
                    TileSelector_SelectedRect = default(Rect);
                    Global.StatusBarTextLeft = "";
                }

                Global.StatusBarTextRight = (TileSelector_Dragging) ? "Selecting tiles " + TileSelector_SelectedRect.ToString() : "";
                if (TileSelector_RectangleSelected != null)
                    Global.StatusBarTextLeft = "Selected tile area " + TileSelector_SelectedRect.ToString() + " on layer "+Global.TileLayer;
            }
        }

        // Entities updates
        private bool Entities_LeftClickDown = false;
        private void Entities_EditorGrid_MouseUpdates(MouseEventArgs e)
        {
            if (EditorGrid.IsMouseOver)
            {
                Global.SelectorIndex = new Point2D((int)e.GetPosition(EditorGrid).X / Global.TileSize, (int)e.GetPosition(EditorGrid).Y / Global.TileSize);

                if (!DraggingEntity)
                {
                    highlightingEntities = Global.Entities.FindAll(t =>
                        (Global.SelectorIndex.X >= t.Position.X && Global.SelectorIndex.X <= t.Position.X + t.Size.X - 1)
                        && (Global.SelectorIndex.Y >= t.Position.Y && Global.SelectorIndex.Y <= t.Position.Y + t.Size.Y - 1)
                    );
                    Global.StatusBarTextRight = (highlightingEntities.Count > 0) ? highlightingEntities[highlightingEntities.Count - 1].Name : "";
                }

                if(!Entities_LeftClickDown && e.LeftButton == MouseButtonState.Pressed)
                {
                    Entities_LeftClickDown = true;
                    foreach (Entity temp in Global.Entities)
                        temp.Selected = false;

                    if (!DraggingEntity && highlightingEntities.Count > 0)
                    {
                        Entity entity = highlightingEntities[highlightingEntities.Count - 1];
                        entity.Selected = true;
                        DraggingOffset = new Point2D(Math.Abs(entity.Position.X - Global.SelectorIndex.X), Math.Abs(entity.Position.Y - Global.SelectorIndex.Y));
                        DraggingEntity = true;
                    }
                }

                if(DraggingEntity)
                {
                    highlightingEntities[highlightingEntities.Count - 1].Selected = true;
                    highlightingEntities[highlightingEntities.Count-1].Position = new Point2D(Global.SelectorIndex.X - DraggingOffset.X, Global.SelectorIndex.Y - DraggingOffset.Y);

                    if (e.LeftButton == MouseButtonState.Released)
                    {
                        Entity newTopMost = highlightingEntities[highlightingEntities.Count - 1];
                        Global.Entities.Remove(newTopMost);
                        Global.Entities.Add(newTopMost);
                        DraggingEntity = false;
                    }
                }

                if (e.LeftButton == MouseButtonState.Released)
                {
                    Entities_LeftClickDown = false;
                }

                if (e.RightButton == MouseButtonState.Pressed)
                {
                    foreach (Entity temp in Global.Entities)
                        temp.Selected = false;

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
        public void NewMap(Point2D mapSize, string texturePath, List<string> layers)
        {
            CloseMap();
            Global.Unsaved = true;
            Global.ResetLayers();
            RemoveAllTiles();

            foreach (string layer in layers)
                Global.AddLayer(layer);

            Global.TileLayer = 1;
            for(int I = 0; I < layers.Count; I++)
            {
                if (layers[I] == Global.Preferences.Read("DefaultSelectedLayer"))
                    Global.TileLayer = I;
            }
            
            Global.TexturePath = texturePath;
            Global.MapSize = mapSize;
            Global.State = States.MapOpen;
            Global.ToolType = ToolTypes.TilePlacer;

            EditorMargin.Width = EditorGrid.Width + 1000;
            EditorMargin.Height = EditorGrid.Height + 1000;
            resetScrollOffset = true;
        }

        public void CloseMap()
        {
            RemoveAllTiles();
            RemoveAllEntities();
            Global.ResetLayers();
            Global.State = States.MapClosed;
            Global.Unsaved = false;
            Global.ToolType = ToolTypes.Null;
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
                TilesetLocation = new Point2D(tilesetPosX, tilesetPosY)
            };

            return tile;
        }

        public void AddTile(Tile tile)
        {
            if (tile.Location.X < 0 || tile.Location.X >= Global.MapSize.X)
                return;
            if (tile.Location.Y < 0 || tile.Location.Y >= Global.MapSize.Y)
                return;

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

        public void RemoveAllEntities()
        {
            for (int i = 0; i < Global.Entities.Count; i++)
                Global.Entities[i].Destroy();
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
            int current = MainWindow.Instance.TileLayerComboBox.SelectedIndex;

            Global.RemoveLayer(current);

            // Move nessesary tiles down a layer
            foreach (var tile in Global.Tiles)
            {
                if (tile.Layer >= current)
                    tile.Layer -= 1;
            }
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

            // Reshuffle
            foreach (var tile in Global.Tiles)
            {
                if (tile.Layer == current)
                    tile.Layer -= 1;
                else if (tile.Layer == current - 1)
                    tile.Layer += 1;
            }
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

            // Reshuffle
            foreach (var tile in Global.Tiles)
            {
                if (tile.Layer == current)
                    tile.Layer += 1;
                else if (tile.Layer == current + 1)
                    tile.Layer -= 1;
            }
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

        private void Preferences_SaveAsCompressed(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            SaveAsCompressed = menuItem.IsChecked;
            Global.StatusBarTextLeft = (SaveAsCompressed) ? "Set save mode to 'Compressed'" : "Set save mode to 'XML'";
        }

        private void Preferences_DefaultLayers(object sender, RoutedEventArgs e)
        {
            DefaultLayers dl = new DefaultLayers();
            dl.ShowDialog();
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

        private void EditorGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Zooming
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = true;

                Scale += (e.Delta > 0) ? 0.1f : -0.1f;

                Global.StatusBarTextLeft = "Zoomed to x" + Scale.ToString();
            }
        }
    }
}