using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace XEditor
{
    public class Tile
    {
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

        public Point TilesetLocation;
        public Rectangle Rectangle;

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
