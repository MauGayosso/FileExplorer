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
            MainWindow winMain = new MainWindow();
            WindowClientsMenu win = new WindowClientsMenu();
            WindowLogin winLogin = new WindowLogin();
            WindowMail winMail = new WindowMail();
            //WindowAyudasVisuales winAyudaV = new WindowAyudasVisuales();
           // winAyudaV.Close();
            winMail.Close();
            winMain.Close();
            winLogin.Close();

            win.Show();
            Close();
        }
    }
}
