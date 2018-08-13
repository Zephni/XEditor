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
    /// Interaction logic for LayerTextInputDialogue.xaml
    /// </summary>
    public partial class LayerTextInputDialogue : Window
    {
        private string LayerName
        {
            get { return LayerText.Text; }
            set { LayerText.Text = value; }
        }

        private string InitialLayerName;

        private bool CreatingNew;

        public LayerTextInputDialogue(bool creatingNew, string defaultLayerName)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LayerName = defaultLayerName;
            InitialLayerName = LayerName;
            CreatingNew = creatingNew;

            if (!CreatingNew)
                this.Title = "Editing layer '" + LayerName + "'";
            else
                this.Title = "Creating new layer";

            LayerText.Focus();
            LayerText.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DefaultLayers.Instance.Layers.Items.Contains(LayerName) && InitialLayerName != LayerName)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show("A layer with this name already exists", "Can't do that!", button);
                return;
            }

            if (CreatingNew)
            {
                if (DefaultLayers.Instance.Layers.SelectedIndex > DefaultLayers.Instance.Layers.Items.Count)
                    DefaultLayers.Instance.Layers.SelectedIndex = DefaultLayers.Instance.Layers.Items.Count;

                DefaultLayers.Instance.Layers.Items.Insert(DefaultLayers.Instance.Layers.SelectedIndex+1, LayerName);
                DefaultLayers.Instance.UpdateChanges();
            }
            else if(!CreatingNew)
            {
                var index = DefaultLayers.Instance.Layers.SelectedIndex;
                DefaultLayers.Instance.Layers.Items.RemoveAt(index);
                DefaultLayers.Instance.Layers.Items.Insert(index, LayerName);
                DefaultLayers.Instance.UpdateChanges();
                DefaultLayers.Instance.DefaultLayer.SelectedIndex = index;
            }

            this.Close();
        }
    }
}
