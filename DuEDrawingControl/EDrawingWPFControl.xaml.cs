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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DuEDrawingControl
{
    /// <summary>
    /// Lógica de interacción para EDrawingWPFControl.xaml
    /// </summary>
    public partial class EDrawingWPFControl : UserControl
    {
        public event Action<dynamic> OnControlLoaded;

        public EDrawingWPFControl()
        {
            InitializeComponent();
            edrawingView.OnControlLoaded += EdrawingView_OnControlLoaded;
        }

        private void EdrawingView_OnControlLoaded(dynamic obj)
        {
            OnControlLoaded?.Invoke(obj);
        }

        /// <summary>
        /// eDrawing host
        /// </summary>
        public EDrawingComponent EDrawingHost { get { return edrawingView.EDrawingHost; } }
    }
}