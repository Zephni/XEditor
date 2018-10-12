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
        bool ForceNew;
        public LevelSettings(bool forceNew = false)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            ForceNew = forceNew;

            if(ForceNew || Global.State != States.MapOpen)
            {
                if (!Global.Preferences.KeyExists("LastUsedMapSize"))
                    Global.Preferences.Write("LastUsedMapSize", "64,64");

                string[] lastUsedMapSize = Global.Preferences.Read("LastUsedMapSize").Split(',');
                GridSize_X.Text = lastUsedMapSize[0];
                GridSize_Y.Text = lastUsedMapSize[1];
                TilesetPath.Text = Global.TexturePath;
                ApplyButton.Content = "Create new level";
                TileSize.IsEnabled = true;
                TileSize.Text = (Global.Preferences.KeyExists("TileSize")) ? Global.Preferences.Read("TileSize") : "16";
            }
            else
            {
                GridSize_X.Text = Global.MapSize.X.ToString();
                GridSize_Y.Text = Global.MapSize.Y.ToString();
                TilesetPath.Text = Global.TexturePath;
                ApplyButton.Content = "Apply changes";
                TileSize.IsEnabled = false;
                TileSize.Text = (Global.Preferences.KeyExists("TileSize")) ? Global.TileSize.ToString() : "16";
            }

            if (Global.Preferences.KeyExists("LastUsedTileset"))
                TilesetPath.Text = Global.Preferences.Read("LastUsedTileset");
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string tilesetPath = TilesetPath.Text;

            try
            {
                int tileSize = Convert.ToInt16(TileSize.Text);
                Global.Preferences.Write("TileSize", tileSize.ToString());
            }
            catch(Exception)
            {
                MessageBox.Show("Tile size must be an integer");
            }
            
            YesNoCancel ync = Global.ImageChecker(tilesetPath, out tilesetPath);

            if (ync == YesNoCancel.Cancel || ync == YesNoCancel.No)
                return;

            if (ForceNew || Global.State != States.MapOpen)
            {
                if(Global.State == States.MapOpen) Global.Command_Close();
                MainWindow.Instance.NewMap(new Point2D(Convert.ToInt32(GridSize_X.Text), Convert.ToInt32(GridSize_Y.Text)), tilesetPath, new List<string>(Global.Preferences.Read("DefaultLayers").Split('|')), Convert.ToInt16(TileSize.Text));
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
                MainWindow.Instance.CloseMap();
                MainWindow.Instance.NewMap(new Point2D(Convert.ToInt32(GridSize_X.Text), Convert.ToInt32(GridSize_Y.Text)), tilesetPath, new List<string>(Global.Preferences.Read("DefaultLayers").Split('|')), Convert.ToInt16(TileSize.Text));

                foreach(var tile in newTiles)
                    tile.TilesetLocation = tile.TilesetLocation; // This refreshes the tileset
                MainWindow.Instance.AddTiles(newTiles);

                foreach (Entity entity in entityList)
                {
                    entity.AddToGrid();
                    Global.Entities.Add(entity);
                }
            }

            Global.Preferences.Write("LastUsedTileset", tilesetPath);
            Global.Preferences.Write("LastUsedMapSize", GridSize_X.Text+","+GridSize_Y.Text);

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
