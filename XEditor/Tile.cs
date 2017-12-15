﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XEditor
{
    public class Tile
    {
        public Rectangle Rectangle = new Rectangle();

        private int layer;
        public int Layer
        {
            get { return layer; }
            set {
                layer = value;
                Rectangle.SetValue(Panel.ZIndexProperty, layer);
            }
        }

        private Point location;
        public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                Rectangle.Margin = new Thickness(value.X * Global.TileSize, value.Y * Global.TileSize, 0, 0);
            }
        }

        private Point tilesetLocation;
        public Point TilesetLocation
        {
            get { return tilesetLocation; }
            set {
                tilesetLocation = value;
                
                Rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                Rectangle.VerticalAlignment = VerticalAlignment.Top;

                if (Global.Bitmap.Width >= ((int)tilesetLocation.X * Global.TileSize) + Global.TileSize
                     && Global.Bitmap.Height >= ((int)tilesetLocation.Y * Global.TileSize) + Global.TileSize)
                {
                    CroppedBitmap cb = new CroppedBitmap(Global.Bitmap, new Int32Rect((int)tilesetLocation.X * Global.TileSize, (int)tilesetLocation.Y * Global.TileSize, Global.TileSize, Global.TileSize));
                    Rectangle.Fill = new ImageBrush(cb);
                }
                else
                {
                    Rectangle.Fill = new ImageBrush();
                }

                Rectangle.Width = Global.TileSize;
                Rectangle.Height = Global.TileSize;
            }
        }

        public Tile DeepCopy()
        {
            return new Tile
            {
                TilesetLocation = TilesetLocation,
                Rectangle = new Rectangle
                {
                    HorizontalAlignment = Rectangle.HorizontalAlignment,
                    VerticalAlignment = Rectangle.VerticalAlignment,
                    Fill = Rectangle.Fill,
                    Width = Rectangle.Width,
                    Height = Rectangle.Height
                },
                Location = Location
            };
        }
    }
}
