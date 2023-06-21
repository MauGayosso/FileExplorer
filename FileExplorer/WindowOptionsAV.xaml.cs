using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.Data.OleDb;
using System.Diagnostics;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowOptionsAV.xaml
    /// </summary>
    public partial class WindowOptionsAV : Window
    {
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f//FileExplorer/attFiles.accdb;";
        List<string> categorias = new List<string>();
        public WindowOptionsAV()
        {
            InitializeComponent();
            getCategorias();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void getCategorias()
        {
            var query = "SELECT * FROM categorias";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.ExecuteNonQuery();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Debug.WriteLine(reader.GetValue(0));
                    comboBox1.Items.Add(reader.GetValue(0));
                    categorias.Add(reader.GetValue(0).ToString());
                }
                reader.Close();
                connection.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.Text == null)
            {
                MessageBox.Show("Selecciona una opcion para continuar", "Advertencia", MessageBoxButton.OK);
            }
            else if (categorias.Contains(comboBox1.Text))
            {
                WindowAyudasVisuales winAV = new WindowAyudasVisuales();
                winAV.parseDirCorte = "C:/Users/mauri/Documents/ARCHIVOS DE AYUDAS VISUALES/"+comboBox1.Text+"/";
                winAV.Show();
                Close();
            }
        }

        private void ShowNotification(string title, string message)
        {
            // Create a new toast notification
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = title
                    },
                    new AdaptiveText()
                    {
                        Text = message
                    }
                }
                    }
                }
            };

            // Create a toast notification object
            ToastNotification notification = new ToastNotification(toastContent.GetXml());

            // Display the notification
            ToastNotificationManagerCompat.CreateToastNotifier().Show(notification);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            WindowLogin win = new WindowLogin();
            win.Show();
            Close();
        }
    }
}