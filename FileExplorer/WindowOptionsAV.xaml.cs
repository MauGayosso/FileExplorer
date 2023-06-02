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
using System.Windows.Media.Animation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowOptionsAV.xaml
    /// </summary>
    public partial class WindowOptionsAV : Window
    {
        public WindowOptionsAV()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox1.Text == null)
            {
                MessageBox.Show("Selecciona una opcion para continuar", "Advertencia", MessageBoxButton.OK);
            }
            else if (comboBox1.Text == "CORTE")
            {
                MainWindow winMain = new MainWindow();
                winMain.Close();
                WindowAyudasVisuales winAV = new WindowAyudasVisuales();
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


    }
}