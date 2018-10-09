using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XEditor
{
    public static class GridDraw
    {
        public static void ClearCanvas(Canvas canvas, Brush backgroundColor = null)
        {
            canvas.Background = (backgroundColor != null) ? backgroundColor : Brushes.White;
        }

        public static void DrawToCanvas(Canvas canvas, int tileSize, Brush strokeColor = null, Brush backgroundColor = null)
        {
            // Build visual brush
            VisualBrush vb = new VisualBrush();
            vb.TileMode = TileMode.Tile;
            vb.Viewport = new Rect(0, 0, tileSize, tileSize);
            vb.Viewbox = new Rect(0, 0, tileSize, tileSize);
            vb.ViewportUnits = BrushMappingMode.Absolute;
            vb.ViewboxUnits = BrushMappingMode.Absolute;

            // Build geometry for lines
            GeometryGroup gg = new GeometryGroup();
            gg.Children.Add(Geometry.Parse("M 0 0 L 0 "+ tileSize.ToString()));
            gg.Children.Add(Geometry.Parse("M 0 0 L "+ tileSize.ToString() + " 0"));
            Path path = new Path { Data = gg, Stroke = (strokeColor != null) ? strokeColor : Brushes.Gray, StrokeThickness=1 };

            // Build grid to fill color and place lines
            Grid grid = new Grid();
            grid.Background = (backgroundColor != null) ? backgroundColor : Brushes.White;
            grid.Children.Add(path);

            // Set visual brush visual to grid
            vb.Visual = grid;

            // Set canvas background to visual brush
            canvas.Background = vb;
        }
    }
}
