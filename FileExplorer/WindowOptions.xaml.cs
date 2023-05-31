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

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowOptions.xaml
    /// </summary>
    public partial class WindowOptions : Window
    {
        public WindowOptions()
        {
            InitializeComponent();
        }

        private void btnClientes_Click(object sender, RoutedEventArgs e)
        {
            WindowClientsMenu win = new WindowClientsMenu();
            win.Show();
            Close();
        }
    }
}
