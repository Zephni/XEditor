using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StaticCoroutines;

namespace XEditor
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            StaticUpdate.Start(Update);
            Selector.Visibility = Visibility.Hidden;
            Global.TileSize = 16;
            Global.Tiles = new List<Tile>();
            Global.MapSize = new Point(64, 64);
        }

        public void Update()
        {
            Coroutines.Update(1 / 60f);
            MouseUpdates(Global.MouseEventArgs);
        }

        public void MouseUpdates(MouseEventArgs e)
        {
            if (EditorGrid.IsMouseOver)
            {
                Selector.Stroke = new SolidColorBrush(Colors.Black);

                Selector.Visibility = Visibility.Visible;
                Global.SelectorIndex = new Point((int)(e.GetPosition(EditorGrid).X / Global.TileSize), (int)(e.GetPosition(EditorGrid).Y / Global.TileSize));

                if(Global.SelectorMode == SelectorModes.Normal)
                {
                    Global.StatusBarTextRight = Global.SelectorIndex.ToString();

                    Selector.Margin = new Thickness(Global.SelectorIndex.X * Global.TileSize, Global.SelectorIndex.Y * Global.TileSize, 0, 0);
                    Selector.Width = Global.TileSize + 1;
                    Selector.Height = Global.TileSize + 1;

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Global.SelectorHoldIndex = new Point(Global.SelectorIndex.X, Global.SelectorIndex.Y);
                        Global.SelectorMode = SelectorModes.SelectingArea;
                        Global.ActionType = ActionTypes.AddingTiles;
                    }

                    if(e.RightButton == MouseButtonState.Pressed)
                    {
                        Global.SelectorHoldIndex = new Point(Global.SelectorIndex.X, Global.SelectorIndex.Y);
                        Global.SelectorMode = SelectorModes.SelectingArea;
                        Global.ActionType = ActionTypes.RemovingTiles;
                    }
                }
                else if(Global.SelectorMode == SelectorModes.SelectingArea)
                {
                    if (Global.ActionType == ActionTypes.AddingTiles)
                        Selector.Stroke = new SolidColorBrush(Colors.Green);
                    else if (Global.ActionType == ActionTypes.RemovingTiles)
                        Selector.Stroke = new SolidColorBrush(Colors.Red);

                    int LeftMost = (int)((Global.SelectorIndex.X >= Global.SelectorHoldIndex.X) ? Global.SelectorHoldIndex.X : Global.SelectorIndex.X);
                    int TopMost = (int)((Global.SelectorIndex.Y >= Global.SelectorHoldIndex.Y) ? Global.SelectorHoldIndex.Y : Global.SelectorIndex.Y);
                    int Width = (int)Math.Abs(Global.SelectorHoldIndex.X - Global.SelectorIndex.X);
                    int Height = (int)Math.Abs(Global.SelectorHoldIndex.Y - Global.SelectorIndex.Y);

                    Selector.Margin = new Thickness(LeftMost * Global.TileSize, TopMost * Global.TileSize, 0, 0);
                    Selector.Width = Width * Global.TileSize + (Global.TileSize + 1);
                    Selector.Height = Height * Global.TileSize + (Global.TileSize + 1);


                    if (Global.ActionType == ActionTypes.AddingTiles)
                        Global.StatusBarTextRight = "Drawing to area " + LeftMost + ", " + TopMost + " (size " + (Width + 1) + ", " + (Height + 1) + ")";
                    else if (Global.ActionType == ActionTypes.RemovingTiles)
                        Global.StatusBarTextRight = "Deleting area " + LeftMost + ", " + TopMost + " (size " + (Width + 1) + ", " + (Height + 1) + ")";

                    if (Global.ActionType == ActionTypes.AddingTiles && e.LeftButton == MouseButtonState.Released)
                    {
                        Global.SelectorMode = SelectorModes.Normal;
                        int tileCount = 0;

                        // Place tile/s
                        for (int X = LeftMost; X <= LeftMost + Width; X++)
                        {
                            for (int Y = TopMost; Y <= TopMost + Height; Y++)
                            {
                                tileCount++;

                                Tile tile = new Tile {
                                    Location = new Point(X, Y)
                                };

                                tile.Rectangle = new Rectangle();
                                tile.Rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                                tile.Rectangle.VerticalAlignment = VerticalAlignment.Top;
                                tile.Rectangle.Fill = new SolidColorBrush(Colors.Blue);
                                tile.Rectangle.Margin = new Thickness(X * Global.TileSize, Y * Global.TileSize, 0, 0);
                                tile.Rectangle.Width = Global.TileSize;
                                tile.Rectangle.Height = Global.TileSize;
                                EditorGrid.Children.Add(tile.Rectangle);
                                Global.Tiles.Add(tile);
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
                                for (int I = 0; I < Global.Tiles.Count; I++)
                                {
                                    if (Global.Tiles[I].Location.X == X && Global.Tiles[I].Location.Y == Y)
                                    {
                                        tileCount++;
                                        EditorGrid.Children.Remove(Global.Tiles[I].Rectangle);
                                        Global.Tiles.Remove(Global.Tiles[I]);
                                    }
                                }
                            }
                        }

                        Global.StatusBarTextLeft = "Removed "+tileCount+" tiles";
                    }
                }
            }
            else
            {
                Selector.Visibility = Visibility.Hidden;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e){Global.MouseEventArgs = e;}
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { Global.MouseEventArgs = e; }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {Global.MouseEventArgs = e; }

        // Menu items
        private void LevelSettings_Click(object sender, RoutedEventArgs e)
        {
            LevelSettings ls = new LevelSettings();
            ls.Show();
        }
    }
}
