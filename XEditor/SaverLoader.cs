﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using XCompressFile;

namespace XEditor
{
    public class SaverLoader
    {
        public void SaveAs(string file)
        {
            XDocument xDoc = DataAsXDocument();

            string finalData = "";

            if (MainWindow.Instance.SaveAsCompressed)
                finalData = Convert.ToBase64String(Compressor.Zip(xDoc.ToString()));
            else
                finalData = xDoc.ToString();

            File.WriteAllText(file, finalData);
            Global.StatusBarTextLeft = "Saved map '" + file + "'";
        }

        public void Load(string file)
        {
            // First try to load as compressed
            try
            {
                string b64CompressedData = File.ReadAllText(file);
                XDocument loadedDoc = XDocument.Parse(Compressor.Unzip(Convert.FromBase64String(b64CompressedData)));
                MainWindow.Instance.CloseMap();
                DataFromDocument(loadedDoc);
                Global.StatusBarTextLeft = "Opened map '"+file+"'";
                //MainWindow.Instance.SaveAsCompressed = true;
            }
            catch(Exception)
            {
                // If fail, then try to load as plain text
                try
                {
                    XDocument loadedDoc = XDocument.Parse(File.ReadAllText(file));
                    MainWindow.Instance.CloseMap();
                    DataFromDocument(loadedDoc);
                    Global.StatusBarTextLeft = "Opened map '" + file + "'";
                    //MainWindow.Instance.SaveAsCompressed = false;
                }
                catch(Exception)
                {
                    MessageBox.Show("Invalid file type '"+file+"'");
                }
            }            
        }

        private XDocument DataAsXDocument()
        {
            XDocument xDoc = new XDocument();
            xDoc.Add(new XElement("Level"));
            
            // EditorConfig
            xDoc.Root.Add(new XElement("EditorConfig",
                new XElement("Tileset", Global.TexturePath),
                new XElement("LayerIndex", Global.TileLayer)
            ));

            // Config
            xDoc.Root.Add(new XElement("Config",
                new XElement("MapSize", Global.MapSize.ToString()),
                new XElement("TileSize", Global.TileSize.ToString()),
                new XElement("Tileset", Global.TexturePath.Split('\\').Last())
            ));

            // Layers
            List<XElement> layers = new List<XElement>();
            foreach (string layer in Global.Layers)
                layers.Add(new XElement(layer));
            xDoc.Root.Element("Config").Add(new XElement("Layers", layers));

            // Entities
            List<object> entities = new List<object>();
            foreach (Entity entity in Global.Entities)
                entities.Add(new XElement("Entity",
                    new XElement("Name", entity.Name),
                    new XElement("Location", entity.Position.X + "," + entity.Position.Y),
                    new XElement("Size", entity.Size.X + "," + entity.Size.Y),
                    new XElement("CustomData", entity.CustomData)
                ));
            xDoc.Root.Add(new XElement("Entities", entities));

            // Tiles
            List<object> tiles = new List<object>();
            foreach (Tile tile in Global.Tiles)
                tiles.Add(new XElement("Tile",
                    new XElement("Location", tile.Location.X + "," + tile.Location.Y + "," + tile.Layer),
                    new XElement("Source", tile.TilesetLocation.X + "," + tile.TilesetLocation.Y)
                ));
            xDoc.Root.Add(new XElement("Tiles", tiles));

            return xDoc;
        }

        private void DataFromDocument(XDocument xDoc)
        {
            string tilesetPath = xDoc.Root.Element("EditorConfig").Element("Tileset").Value;

            YesNoCancel ync = Global.ImageChecker(tilesetPath, out tilesetPath);

            if (ync == YesNoCancel.Cancel || ync == YesNoCancel.No)
                return;

            string[] mapSize = xDoc.Root.Element("Config").Element("MapSize").Value.Split(',');
            Point2D mapSizep = new Point2D(Convert.ToInt16(mapSize[0]), Convert.ToInt16(mapSize[1]));

            int tileSize = Convert.ToInt16(xDoc.Root.Element("Config").Element("TileSize").Value);

            List<string> layers = new List<string>();
            foreach(XElement xEl in xDoc.Root.Element("Config").Elements("Layers").Descendants())
                layers.Add(xEl.Name.ToString());

            MainWindow.Instance.NewMap(mapSizep, tilesetPath, layers, tileSize);

            foreach (XElement xEl in xDoc.Root.Element("Tiles").Elements("Tile"))
            {
                string[] source = xEl.Element("Source").Value.Split(',');
                string[] location = xEl.Element("Location").Value.Split(',');

                Tile tile = new Tile
                {
                    TilesetLocation = new Point2D(Convert.ToInt16(source[0]), Convert.ToInt16(source[1])),
                    Location = new Point2D(Convert.ToInt16(location[0]), Convert.ToInt16(location[1])),
                    Layer = Convert.ToInt16(location[2])
                };

                MainWindow.Instance.AddTile(tile);
            }

            foreach (XElement xEl in xDoc.Root.Element("Entities").Elements("Entity"))
            {
                string[] location = xEl.Element("Location").Value.Split(',');
                string[] size = xEl.Element("Size").Value.Split(',');

                Entity entity = new Entity
                {
                    Name = xEl.Element("Name").Value,
                    Position = new Point2D(Convert.ToInt16(location[0]), Convert.ToInt16(location[1])),
                    Size = new Point2D(Convert.ToInt16(size[0]), Convert.ToInt16(size[1])),
                    CustomData = xEl.Element("CustomData").Value
                };

                Global.Entities.Add(entity);
            }
            
            Global.ToolType = ToolTypes.TilePlacer;

            Global.TileLayer = Convert.ToInt16(xDoc.Root.Element("EditorConfig").Element("LayerIndex").Value);
        }
    }
}
