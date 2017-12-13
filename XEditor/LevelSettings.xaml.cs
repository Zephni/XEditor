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
using System.Windows.Shapes;

namespace XEditor
{
    /// <summary>
    /// Interaction logic for LevelSettings.xaml
    /// </summary>
    public partial class LevelSettings : Window
    {
        public LevelSettings()
        {
            InitializeComponent();

            GridSize_X.Text = Global.MapSize.X.ToString();
            GridSize_Y.Text = Global.MapSize.Y.ToString();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Global.MapSize = new Point(
                Convert.ToInt32(GridSize_X.Text),
                Convert.ToInt32(GridSize_Y.Text)
            );

            Global.TexturePath = TilesetPath.Text;

            this.Close();
        }

        private void Browse_Tileset(object sender, RoutedEventArgs e)
        {
            ;Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = TilesetPath.Text;
            ofd.RestoreDirectory = true;
            if(ofd.ShowDialog(this) == true)
            {
                TilesetPath.Text = ofd.FileName;
                TilesetPath.ScrollToEnd();
            }
        }
    }
}
