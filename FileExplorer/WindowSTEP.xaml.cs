using netDxf.Entities;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Xbim.Common.Geometry;
using Xbim.Geometry.Engine;
using Xbim.Geometry.Engine.Interop;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using Xbim.Presentation;
using HelixToolkit.Wpf;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowSTEP.xaml
    /// </summary>
    public partial class WindowSTEP : Window
    {
        public WindowSTEP()
        {
            InitializeComponent();
        }
        public void LoadStepFile(string pathStep)
        {
            var model = IfcStore.Open(pathStep);
            using (model)
            {

                var geometryStore = new XbimGeometryEngine();
                var context = new Xbim3DModelContext(model);
               
            }
        }

    }
}
