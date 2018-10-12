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
    /// Interaction logic for EntitySettings.xaml
    /// </summary>
    public partial class EntitySettings : Window
    {
        public string ApplyButtonText
        {
            set
            {
                ApplyButton.Content = value;
            }
        }

        private Entity editingEntity;
        public Entity EditingEntity
        {
            get { return editingEntity; }
            set
            {
                editingEntity = value;
                _EntityName = editingEntity.Name;
                _PosX = editingEntity.Position.X.ToString();
                _PosY = editingEntity.Position.Y.ToString();
                _SizeX = editingEntity.Size.X.ToString();
                _SizeY = editingEntity.Size.Y.ToString();
                _CustomData = editingEntity.CustomData.ToString();
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        public string _EntityName
        {
            get { return EntityName.Text; }
            set { EntityName.Text = value; }
        }
        public string _PosX
        {
            get { return PosX.Text; }
            set { PosX.Text = value; }
        }
        public string _PosY
        {
            get { return PosY.Text; }
            set { PosY.Text = value; }
        }
        public string _SizeX
        {
            get { return SizeX.Text; }
            set { SizeX.Text = value; }
        }
        public string _SizeY
        {
            get { return SizeY.Text; }
            set { SizeY.Text = value; }
        }
        public string _CustomData
        {
            get { return CustomData.Text; }
            set { CustomData.Text = value; }
        }

        public EntitySettings()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            EntityName.SelectAll();
            DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if(EditingEntity == null)
            {
                Entity entity = new Entity
                {
                    Name = _EntityName,
                    Position = new Point2D(Convert.ToInt16(_PosX), Convert.ToInt16(_PosY)),
                    Size = new Point2D(Convert.ToInt16(_SizeX), Convert.ToInt16(_SizeY)),
                    CustomData = _CustomData
                };

                Global.Entities.Add(entity);
                entity.Selected = true;
                entity.Rectangle.ToolTip = entity.Name;
                Global.StatusBarTextLeft = "Created entity '"+entity.Name+"'";
            }
            else
            {
                editingEntity.Name = _EntityName;
                editingEntity.Position = new Point2D(Convert.ToInt16(_PosX), Convert.ToInt16(_PosY));
                editingEntity.Size = new Point2D(Convert.ToInt16(_SizeX), Convert.ToInt16(_SizeY));
                editingEntity.CustomData = _CustomData;
                editingEntity.Rectangle.ToolTip = _EntityName;
                Global.StatusBarTextLeft = "Edited entity '" + EditingEntity.Name + "'";
            }

            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditingEntity != null)
            {
                Global.StatusBarTextLeft = "Deleted entity '" + EditingEntity.Name + "'";
                EditingEntity.Destroy();
                Close();
            }
        }
    }
}
