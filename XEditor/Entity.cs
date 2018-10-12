using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XEditor
{
    public class Entity
    {
        private string name;
        public string Name {
            get { return name; }
            set
            {
                Global.Unsaved = true;
                name = value;
            }
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;

                if(selected == false)
                {
                    Rectangle.Fill = Brushes.LightBlue;
                    Rectangle.Stroke = Brushes.DarkBlue;
                    Rectangle.StrokeThickness = 2;
                    Rectangle.StrokeDashArray = new DoubleCollection { 1 };
                    Rectangle.SetValue(Panel.ZIndexProperty, 100);
                    Rectangle.Opacity = 0.4f;
                }
                else
                {
                    Rectangle.Fill = Brushes.AntiqueWhite;
                    Rectangle.Stroke = Brushes.OrangeRed;
                    Rectangle.StrokeThickness = 2;
                    Rectangle.StrokeDashArray = new DoubleCollection { 1 };
                    Rectangle.Opacity = 0.8f;
                    Rectangle.SetValue(Panel.ZIndexProperty, 101);
                }
            }
        }

        private string customData;
        public string CustomData
        {
            get { return customData; }
            set
            {
                Global.Unsaved = true;
                customData = value;
            }
        }

        private Point2D position;
        public Point2D Position
        {
            get { return position; }
            set
            {
                Global.Unsaved = true;
                position = value;

                if (position.X + size.X > Global.MapSize.X)
                    position.X = Global.MapSize.X - size.X;
                else if (position.X < 0)
                    position.X = 0;
                if (position.Y + size.Y > Global.MapSize.Y)
                    position.Y = Global.MapSize.Y - size.Y;
                else if (position.Y < 0)
                    position.Y = 0;

                Rectangle.Margin = new System.Windows.Thickness(
                    position.X * Global.TileSize,
                    position.Y * Global.TileSize,
                    0, 0
                );
            }
        }

        private Point2D size;
        public Point2D Size
        {
            get { return size; }
            set
            {
                Global.Unsaved = true;
                size = value;
                Rectangle.Width = size.X * Global.TileSize;
                Rectangle.Height = size.Y * Global.TileSize;
            }
        }

        public Rectangle Rectangle;

        public Entity()
        {
            Rectangle = new Rectangle();
            Rectangle.Fill = Brushes.Black;
            Rectangle.Opacity = 0.8f;
            Rectangle.SetValue(Panel.ZIndexProperty, 100);
            Selected = false;
            AddToGrid();
        }

        public void AddToGrid()
        {
            MainWindow.Instance.EditorGrid.Children.Add(Rectangle);
        }

        public void Destroy()
        {
            MainWindow.Instance.EditorGrid.Children.Remove(Rectangle);
            Global.Entities.Remove(this);
        }
    }
}
