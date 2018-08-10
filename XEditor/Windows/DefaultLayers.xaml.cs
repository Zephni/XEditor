using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XEditor
{
    /// <summary>
    /// Interaction logic for DefaultLayers.xaml
    /// </summary>
    public partial class DefaultLayers : Window
    {
        public DefaultLayers()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;



            Layers.Text = String.Join(Environment.NewLine, Global.Preferences.Read("DefaultLayers").Split(new string[] { "|" }, StringSplitOptions.None));
            DefaultLayer.Text = Global.Preferences.Read("DefaultSelectedLayer");
        }

        private void UpdateDefaultLayers(object sender, RoutedEventArgs e)
        {
            Global.Preferences.Write("DefaultLayers", String.Join("|", Layers.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)));
            Global.Preferences.Write("DefaultSelectedLayer", DefaultLayer.Text);
            this.Close();
        }
    }
}
