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
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin : Window
    {
        string pathDB = "C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/clientes.db";
        SQLiteCommand command;
        SQLiteConnection connection = new SQLiteConnection($"Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/clientes.db;Version=3");
        SQLiteDataReader reader;

        public WindowLogin()
        {
            InitializeComponent();
            connection.Open();
        }

        private void txtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsuario.Text;
            string password = txtPassword.Password;
            Debug.WriteLine("USERNAME : " + username);
            Debug.WriteLine("PASSWORD: " + password);
            VerificarCredenciales(username, password);


        }


        private void VerificarCredenciales(string username, string password)
        {
            string nameMachine = System.Environment.MachineName;
            string logFilePath = "C:/Users/mauri/OneDrive/Escritorio/log.txt";
            Debug.WriteLine("Name Machine : " + nameMachine);
            using (StreamWriter write = File.AppendText(logFilePath))
            {
                write.WriteLine($"[{DateTime.Now}, {nameMachine},{username}]");
            }
            string query = "SELECT COUNT(*) FROM Usuarios WHERE username=@username AND password=@password";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                int count = Convert.ToInt32(command.ExecuteScalar());
                
                if(count > 0) 
                {
                    WindowClientsMenu win = new WindowClientsMenu();
                    win.Show();
                    Close();
                }
                else
                {
                    System.Windows.MessageBox.Show("Usuario y/o Contraseña Invalidos", "Error", MessageBoxButton.OK);
                }

            }
        }

        private void btnVisitante_Click(object sender, RoutedEventArgs e)
        {
            string nameMachine = System.Environment.MachineName;
            string logFilePath = "C:/Users/mauri/OneDrive/Escritorio/log.txt";
            Debug.WriteLine("Name Machine : " + nameMachine);
            using (StreamWriter write = File.AppendText(logFilePath))
            {
                write.WriteLine($"[{DateTime.Now}, {nameMachine}]");
            }
            WindowAyudasVisuales win = new WindowAyudasVisuales();
            win.Show();
            Close();
        }
    }
}
