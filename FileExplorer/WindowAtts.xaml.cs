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
using System.Data.OleDb;
namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowAtts.xaml
    /// </summary>
    public partial class WindowAtts : Window
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/attFiles.accdb");
        public WindowAtts()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OleDbCommand cmd = con.CreateCommand();
            cmd.CommandText = "insert into atts (Id_file,revision,materia_prima) values (@id,@rev,@matp)";
            using (con)
            {
                try
                {
                    con.Open();

                    // Create an INSERT query
                    string insertQuery = "insert into atts (Id_file,revision,materia_prima) values (@id,@rev,@matp)";

                    // Create a command object with the query and connection
                    using (OleDbCommand command = new OleDbCommand(insertQuery, con))
                    {
                        // Add parameter values
                        command.Parameters.AddWithValue("@Value1", Node.selectedBytes);
                        command.Parameters.AddWithValue("@Value2", txtRevision.Text);
                        command.Parameters.AddWithValue("@Value3", txtMateria.Text);

                        // Execute the query
                        command.ExecuteNonQuery();

                        MessageBox.Show("Data inserted successfully!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
