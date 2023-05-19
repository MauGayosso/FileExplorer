using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Lógica de interacción para WindowClientsMenu.xaml
    /// </summary>
    public partial class WindowClientsMenu : Window
    {
        public WindowClientsMenu()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            if(comboBox1.Text == null)
            {
                MessageBox.Show("Selecciona una opcion para continuar","Advertencia",MessageBoxButton.OK);
            }
            else if (comboBox1.Text == "Cliente 1")
            {
         
                MainWindow win = new MainWindow();
                win.Show();
                Close();
            }
        }
    }
}
