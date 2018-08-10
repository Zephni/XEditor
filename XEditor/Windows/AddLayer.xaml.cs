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
    public partial class AddLayer : Window
    {
        public AddLayer()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            foreach (string layerAlias in Global.Layers)
                LayerIndexes.Items.Add(layerAlias);

            LayerIndexes.SelectedIndex = MainWindow.Instance.TileLayerComboBox.SelectedIndex;

            // Move nessesary tiles up a layer
            foreach(var tile in Global.Tiles)
            {
                if (tile.Layer >= LayerIndexes.SelectedIndex)
                    tile.Layer += 1;
            }

            LayerName.Focus();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (Global.AddLayer(LayerName.Text, LayerIndexes.SelectedIndex))
                this.Close();
        }
    }
}
