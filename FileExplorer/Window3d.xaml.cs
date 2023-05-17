using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para Window3D.xaml
    /// </summary>
    public partial class Window3D : Window
    {
        public static Window3D instance;
        Window win = new Window();
        ModelVisual3D device3d = new ModelVisual3D();
        public Window3D()
        {
            InitializeComponent();
            instance = this;

        }
        public Model3D Display3d(string model)
        {
            Debug.WriteLine("MODEL : " + model);
            Model3D device = null;
            try
            {
                //Adding a gesture here
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                //Load the 3D model file
                device = import.Load(model);
            }
            catch (Exception e)
            {
                // Handle exception in case can not file 3D model
                System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);

            }
            device3d.Content = device;
            viewPort3d.Children.Add(device3d);

            return null;
        }
       

    }
}
