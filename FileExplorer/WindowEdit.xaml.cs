using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
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
using Windows.Storage.Provider;
using Path = System.IO.Path;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowEdit.xaml
    /// </summary>
    public partial class WindowEdit : Window
    {

        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/attFiles.accdb;";
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/attFiles.accdb");
        public string pathPass;
        public WindowEdit()
        {
            InitializeComponent();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            editDat(pathPass);
            Debug.WriteLine("PathPass: " + pathPass);
        }
        public void loadName(string name)
        {
            var n = Path.GetFileName(name);
            txtName.Text = Path.GetFileName(name);
            Debug.WriteLine("Name: " + n);
        }
        public void atts(string id)
        {
            var query = "SELECT * FROM atts WHERE Id_file=@Value1";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Value1", id);
                command.ExecuteNonQuery();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtRevision.Text = reader.GetValue(1).ToString();
                    txtMateria.Text = reader.GetValue(2).ToString();
                }
                reader.Close();
                connection.Close();
            }
        }
        public void editDat(string path)
        {
            string query = "UPDATE atts SET revision = ?, materia_prima = ? WHERE Id_file = ?";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@revisionValue", txtRevision.Text);
                    command.Parameters.AddWithValue("@materia_primaValue", txtMateria.Text);
                    command.Parameters.AddWithValue("@Id_fileValue", path);

                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Actualizado correctamente","Ok",MessageBoxButton.OK,MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar valor","Error",MessageBoxButton.OK,MessageBoxImage.Warning);
                    }
                }
            }
        }
    }
}
