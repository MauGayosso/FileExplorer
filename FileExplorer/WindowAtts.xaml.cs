using System;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowAtts.xaml
    /// </summary>
    public partial class WindowAtts : Window
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/attFiles.accdb");
        public string pathPass;
        public WindowAtts()
        {
            InitializeComponent();
            loadName(pathPass);
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            addDat(pathPass);
            Debug.WriteLine("PathPass: " + pathPass);
        }

        public void loadName(string name)
        {
            var n = Path.GetFileName(name);
            txtName.Text = Path.GetFileName(name);
            Debug.WriteLine("Name: " + n);
        }

        public void addDat(string path)
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
                        command.Parameters.AddWithValue("@Value1", path);
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
