using System.Windows.Media;
using System.Windows.Shapes;

namespace XEditor
{
    public class Entity
    {
        public string Name { get; set; }

        public string CustomData { get; set; }

        private Point position;
        public Point Position
        {
            get { return position; }
            set
            {
                position = value;
                Rectangle.Margin = new System.Windows.Thickness(
                    position.X * Global.TileSize,
                    position.Y * Global.TileSize,
                    0, 0
                );
            }
        }

        private Point size;
        public Point Size
        {
            get { return size; }
            set
            {
                size = value;
                Rectangle.Width = size.X * Global.TileSize;
                Rectangle.Height = size.Y * Global.TileSize;
            }
        }

        private Rectangle Rectangle;

        public Entity()
        {
            Rectangle = new Rectangle();
            Rectangle.Fill = new SolidColorBrush(new Color { R = 50, G = 50, B = 230, A = 100 });
            MainWindow.Instance.EditorGrid.Children.Add(Rectangle);
        }

        public void Destroy()
        {
            MainWindow.Instance.EditorGrid.Children.Remove(Rectangle);
            Global.Entities.Remove(this);
        }
    }
}
