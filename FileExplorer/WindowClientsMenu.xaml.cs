
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;


namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowClientsMenu.xaml
    /// </summary>
    public partial class WindowClientsMenu : Window
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f/FileExplorer/MI_DB/attFiles.accdb");
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f//FileExplorer/MI_DB/attFiles.accdb;";
        List<string> clientes = new List<string>();
        public WindowClientsMenu()
        {
            InitializeComponent();
            getClientes();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void getClientes()
        {
            var query = "SELECT * FROM clientes";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.ExecuteNonQuery();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetValue(0));
                    clientes.Add(reader.GetValue(0).ToString());
                }
                reader.Close();
                connection.Close();
            }
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.Text == null)
            {
                MessageBox.Show("Selecciona una opcion para continuar", "Advertencia", MessageBoxButton.OK);
            }
            else if (clientes.Contains(comboBox1.Text))
            {
                MainWindow win = new MainWindow();
                win.parseDir = "C:/Users/mauri/Documents/ING/"+comboBox1.Text+"/";
                win.ParseNewDir();
                win.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Cliente no encontrado");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            WindowOptions win = new WindowOptions();
            win.Show();
            Close();
        }
    }
}
