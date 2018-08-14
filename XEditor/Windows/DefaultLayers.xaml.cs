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
        public static DefaultLayers Instance;

        public DefaultLayers()
        {
            if (Instance == null)
                Instance = this;

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            foreach(var item in Global.Preferences.Read("DefaultLayers").Split(new string[] { "|" }, StringSplitOptions.None))
                Layers.Items.Add(item);

            Layers.SelectedIndex = 0;

            UpdateChanges();
        }

        public void UpdateDefaultLayers()
        {
            List<string> layerStrList = new List<string>();
            foreach (var item in Layers.Items)
                layerStrList.Add(item.ToString());

            string defaultLayersString = String.Join("|", layerStrList);

            Global.Preferences.Write("DefaultLayers", defaultLayersString);
            Global.Preferences.Write("DefaultSelectedLayer", DefaultLayer.Text);
        }

        public void UpdateChanges()
        {
            // Default layer
            var current = DefaultLayer.Text;
            DefaultLayer.Items.Clear();
            foreach (var item in Layers.Items)
            {
                DefaultLayer.Items.Add(item.ToString());
                if (item.ToString() == current)
                    DefaultLayer.SelectedItem = item;
            }

            if (DefaultLayer.SelectedItem == null || DefaultLayer.SelectedItem.ToString() == "")
                DefaultLayer.SelectedItem = DefaultLayer.Items[0];
        }

        private void ContextMenu_Add(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex == -1)
                return;

            LayerTextInputDialogue ltid = new LayerTextInputDialogue(true, "NewLayer");
            ltid.Show();
            UpdateDefaultLayers();
        }
        private void ContextMenu_Edit(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex == -1)
                return;

            LayerTextInputDialogue ltid = new LayerTextInputDialogue(false, Layers.SelectedItem.ToString());
            ltid.Show();
            UpdateDefaultLayers();
        }
        private void ContextMenu_Delete(object sender, RoutedEventArgs e)
        {
            if (Layers.SelectedIndex == -1)
                return;

            if (Layers.Items.Count == 1)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show("There must be at least one layer", "Can't do that!", button);
                return;
            }

            Layers.Items.Remove(Layers.SelectedItem);
            UpdateChanges();
            UpdateDefaultLayers();
        }
        private void ContextMenu_MoveUp(object sender, RoutedEventArgs e)
        {
            MoveItem(-1);
        }

        private void ContextMenu_MoveDown(object sender, RoutedEventArgs e)
        {
            MoveItem(1);
        }

        public void MoveItem(int direction)
        {
            // Checking selected item
            if (Layers.SelectedItem == null || Layers.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = Layers.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= Layers.Items.Count)
                return; // Index out of range - nothing to do

            object selected = Layers.SelectedItem;

            // Removing removable element
            Layers.Items.Remove(selected);
            // Insert it in new position
            Layers.Items.Insert(newIndex, selected);
            // Restore selection
            Layers.SelectedIndex = newIndex;

            UpdateDefaultLayers();
        }

        private void Layers_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Layers.UnselectAll();
        }
    }
}
