﻿using System;
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
        }

        // MouseUpdates
        public void MouseUpdates(MouseEventArgs e)
        {
            if (Global.State == States.MapOpen)
            {
                EditorGridMouseUpdates(e);
                TilesetMouseUpdates(e);
            }
        }

        // Methods
        public void NewMap(Point mapSize, string texturePath)
        {
            Global.TexturePath = texturePath;
            Global.MapSize = mapSize;
            Global.State = States.MapOpen;
        }

        public void CloseMap()
        {
            for (int x = 0; x < Global.Tiles.GetLength(0); x++)
                for (int y = 0; y < Global.Tiles.GetLength(1); y++)
                    RemoveTile(x, y);

            Global.State = States.MapClosed;
        }

        public void OpenMap(Point mapSize, string texturePath, List<Tile> tiles)
        {
            CloseMap();
            NewMap(mapSize, texturePath);

            foreach (Tile tile in tiles)
            {
                tile.Location = tile.Location;
                tile.TilesetLocation = tile.TilesetLocation;
            }

            AddTiles(tiles);
        }

        public void EditorGridMouseUpdates(MouseEventArgs e)
        {
            Rectangle ThisSelector = Selector;

            if (EditorGrid.IsMouseOver)
            {
                ThisSelector.Stroke = new SolidColorBrush(Colors.Black);

                ThisSelector.Visibility = Visibility.Visible;
                Global.SelectorIndex = new Point((int)(e.GetPosition(EditorGrid).X / Global.TileSize), (int)(e.GetPosition(EditorGrid).Y / Global.TileSize));

                if (Global.SelectorMode == SelectorModes.Normal)
                {
                    Global.StatusBarTextRight = Global.SelectorIndex.ToString();

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
                                RemoveTile(x, y);

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
                                if(RemoveTile(X, Y))
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

                    if (TopMost + Height > Math.Floor(Global.Bitmap.Height / Global.TileSize))
                        Height--;
                    if (LeftMost + Width > Math.Floor(Global.Bitmap.Width / Global.TileSize))
                        Width--;

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
            EditorGrid.Children.Add(tile.Rectangle);
            Global.Tiles[(int)tile.Location.X, (int)tile.Location.Y] = tile;
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
                for(int x = 0; x < Global.Tiles.GetLength(0); x++)
                    for (int y = 0; y < Global.Tiles.GetLength(1); y++)
                        if (Global.Tiles[x, y] != null)
                            tiles.Add(Global.Tiles[x, y]);
            }

            return tiles;
        }

        public bool RemoveTile(int x, int y)
        {
            if (Global.Tiles[x, y] == null)
                return false;

            EditorGrid.Children.Remove(Global.Tiles[x, y].Rectangle);
            Global.Tiles[x, y] = null;
            return true;
        }

        public void RemoveAllTiles()
        {
            if (Global.Tiles == null)
                return;

            for (int x = 0; x < Global.Tiles.GetLength(0); x++)
                for (int y = 0; y < Global.Tiles.GetLength(1); y++)
                    RemoveTile(x, y);
        }

        // Window events
        private void Window_MouseMove(object sender, MouseEventArgs e){Global.MouseEventArgs = e; MouseUpdates(e); }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { Global.MouseEventArgs = e; MouseUpdates(e); }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {Global.MouseEventArgs = e; MouseUpdates(e); }

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
    }
}