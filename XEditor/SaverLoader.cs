using System;
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
            string b64CompressedData = Convert.ToBase64String(Compressor.Zip(xDoc.ToString()));
            File.WriteAllText(file, b64CompressedData);
        }

        public void Load(string file)
        {
            try
            {
                string b64CompressedData = File.ReadAllText(file);
                XDocument loadedDoc = XDocument.Parse(Compressor.Unzip(Convert.FromBase64String(b64CompressedData)));
                MainWindow.Instance.CloseMap();
                DataFromDocument(loadedDoc);
            }
            catch(Exception e)
            {
                MessageBox.Show("Invalid file type");
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
                new XElement("Tileset", Global.TexturePath.Split('\\').Last())
            ));

            // Layers
            List<XElement> layers = new List<XElement>();
            foreach (string layer in Global.Layers)
                layers.Add(new XElement(layer));
            xDoc.Root.Element("Config").Add(new XElement("Layers", layers));

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
            Point mapSizep = new Point(Convert.ToInt16(mapSize[0]), Convert.ToInt16(mapSize[1]));

            List<string> layers = new List<string>();
            foreach(XElement xEl in xDoc.Root.Element("Config").Elements("Layers").Descendants())
                layers.Add(xEl.Name.ToString());

            List<Tile> tiles = new List<Tile>();
            foreach (XElement xEl in xDoc.Root.Element("Tiles").Elements("Tile"))
            {
                string[] source = xEl.Element("Source").Value.Split(',');
                string[] location = xEl.Element("Location").Value.Split(',');

                Tile tile = new Tile
                {
                    TilesetLocation = new Point(Convert.ToInt16(source[0]), Convert.ToInt16(source[1])),
                    Location = new Point(Convert.ToInt16(location[0]), Convert.ToInt16(location[1])),
                    Layer = Convert.ToInt16(location[2])
                };

                tiles.Add(tile);
            }
            
            MainWindow.Instance.OpenMap(mapSizep, tilesetPath, layers, tiles);

            Global.TileLayer = Convert.ToInt16(xDoc.Root.Element("EditorConfig").Element("LayerIndex").Value);
        }
    }
}
