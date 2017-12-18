using System;
using System.Collections.Generic;
using System.Windows;

namespace XEditor
{
    /// <summary>
    /// Interaction logic for LevelSettings.xaml
    /// </summary>
    public partial class LevelSettings : Window
    {
        public LevelSettings()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            if(Global.State != States.MapOpen)
            {
                GridSize_X.Text = "64";
                GridSize_Y.Text = "64";
                TilesetPath.Text = Global.TexturePath;
                ApplyButton.Content = "Create new level";
            }
            else
            {
                GridSize_X.Text = Global.MapSize.X.ToString();
                GridSize_Y.Text = Global.MapSize.Y.ToString();
                TilesetPath.Text = Global.TexturePath;
                ApplyButton.Content = "Apply changes";
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string tilesetPath = TilesetPath.Text;
            YesNoCancel ync = Global.ImageChecker(tilesetPath, out tilesetPath);

            if (ync == YesNoCancel.Cancel || ync == YesNoCancel.No)
                return;

            if (Global.State != States.MapOpen)
            {
                MainWindow.Instance.NewMap(new Point(Convert.ToInt32(GridSize_X.Text), Convert.ToInt32(GridSize_Y.Text)), tilesetPath, new List<string> { "Background", "Main", "Foreground" });
            }
            else
            {
                List<Tile> newTiles = MainWindow.Instance.GetTileList();
                List<Entity> entityList = new List<Entity>();
                for (int i = 0; i < Global.Entities.Count; i++)
                {
                    entityList.Add(Global.Entities[i]);
                    Global.Entities[i].Destroy();
                }
                    
                // Needs fixing
                MainWindow.Instance.NewMap(new Point(Convert.ToInt32(GridSize_X.Text), Convert.ToInt32(GridSize_Y.Text)), tilesetPath, new List<string> { "Background", "Main", "Foreground" });
                MainWindow.Instance.AddTiles(newTiles);
                foreach(Entity entity in entityList)
                {
                    entity.AddToGrid();
                    Global.Entities.Add(entity);
                }
            }

            this.Close();
        }

        private void Browse_Tileset(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = TilesetPath.Text;
            ofd.RestoreDirectory = true;

            if(ofd.ShowDialog(this) == true)
            {
                string tilesetPath = ofd.FileName;

                TilesetPath.Text = tilesetPath;
                TilesetPath.ScrollToEnd();
            }
        }
    }
}
