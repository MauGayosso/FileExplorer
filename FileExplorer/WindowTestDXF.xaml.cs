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
using netDxf;
using netDxf.Entities;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowTestDXF.xaml
    /// </summary>
    public partial class WindowTestDXF : Window
    {
        public WindowTestDXF()
        {
            InitializeComponent();  
        }
        /*
        private void LoadFileDXF(string pathDXF, System.Windows.Shapes.Shape shape)
        {
            DxfDocument dxf = DxfDocument.Load(pathDXF);
            foreach (EntityObject entity in dxf.Entities)
            {
                netDxf.Entities.Shape shape = CreateShapeFromEntity(entity, GetShape());
                canvas.Children.Add(shape);
            }
        }

        private netDxf.Entities.Shape GetShape()
        {
            return shape;
        }

        private netDxf.Entities.Shape CreateShapeFromEntity(EntityObject entity, System.Windows.Shapes.Shape shape)
        {
         
            if (entity is netDxf.Entities.Line line)
            {
                // Crea una línea en base a los puntos de inicio y fin
                shape = new System.Windows.Shapes.Line
                {
                    X1 = line.StartPoint.X,
                    Y1 = line.StartPoint.Y,
                    X2 = line.EndPoint.X,
                    Y2 = line.EndPoint.Y,
                    Stroke = Brushes.Black
                };
            }
            else if (entity is Circle circle)
            {
                // Crea un círculo en base al centro y al radio
                shape = new System.Windows.Shapes.Ellipse
                {
                    Width = circle.Radius * 2,
                    Height = circle.Radius * 2,
                    Stroke = Brushes.Black
                };

                Canvas.SetLeft(shape, circle.Center.X - circle.Radius);
                Canvas.SetTop(shape, circle.Center.Y - circle.Radius);
            }
            else
            {
                // Otros tipos de entidades no soportados en este ejemplo
                shape = null;
            }

            return shape;
        }*/
    }
}
