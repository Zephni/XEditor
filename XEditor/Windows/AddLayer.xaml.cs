﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XEditor
{
    /// <summary>
    /// Interaction logic for AddLayer.xaml
    /// </summary>
    public partial class AddEditLayer : Window
    {
        public enum Mode
        {
            Create,
            Edit
        }

        Mode CurrentMode;
        private string OriginalName = "";

        public AddEditLayer(Mode mode)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            CurrentMode = mode;

            ApplyButton.Content = (CurrentMode == Mode.Create) ? "Create layer" : "Edit layer";
            this.Title = (CurrentMode == Mode.Create) ? "Creating layer" : "Editing layer";
            if (CurrentMode == Mode.Edit)
            {
                OriginalName = MainWindow.Instance.TileLayerComboBox.SelectedItem.ToString();
                LayerName.Text = OriginalName;
                LayerName.Focus();
                LayerName.SelectAll();
                PositionLabel.IsEnabled = false;
                LayerIndexes.IsEnabled = false;
            }

            foreach (string layerAlias in Global.Layers)
                LayerIndexes.Items.Add(layerAlias);

            LayerIndexes.SelectedIndex = MainWindow.Instance.TileLayerComboBox.SelectedIndex;

            LayerName.Focus();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (LayerName.Text.Length == 0)
            {
                MessageBoxButton mbb = MessageBoxButton.OK;
                MessageBox.Show("A layer must be at least 1 character", "Can't do that!", mbb);
                return;
            }

            if (LayerName.Text.Contains(' '))
            {
                MessageBoxButton mbb = MessageBoxButton.OK;
                MessageBox.Show("A layer cannot contain any spaces", "Can't do that!", mbb);
                return;
            }

            if (OriginalName != LayerName.Text && Global.Layers.Contains(LayerName.Text))
            {
                MessageBox.Show("A layer with this name already exists");
                return;
            }

            if (CurrentMode == Mode.Create)
            {
                // Move nessesary tiles up a layer
                foreach (var tile in Global.Tiles)
                {
                    if (tile.Layer >= LayerIndexes.SelectedIndex)
                        tile.Layer += 1;
                }

                if (Global.AddLayer(LayerName.Text, LayerIndexes.SelectedIndex))
                    this.Close();
            }
            else if(CurrentMode == Mode.Edit)
            {
                var index = MainWindow.Instance.TileLayerComboBox.SelectedIndex;
                Global.Layers.RemoveAt(index);
                Global.Layers.Insert((index - 1 >= 0) ? index-1 : 0, LayerName.Text);
                this.Close();
            }
        }
    }
}
