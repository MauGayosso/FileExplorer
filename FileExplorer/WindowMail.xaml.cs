using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using Path = System.IO.Path;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Diagnostics;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowMail.xaml
    /// </summary>
    public partial class WindowMail : Window
    {
        public WindowMail()
        {
            InitializeComponent();
        }
        List<string> FileLists = new List<string>();
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            mailItem.Subject = txtSubject.Text;
            mailItem.Body = txtBody.Text;
            mailItem.To = txtTO.Text;
            foreach(var file in FileLists)
            {
                mailItem.Attachments.Add(file);
            }
            mailItem.Send();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // Allow multiple file selection
            openFileDialog.InitialDirectory = @"C\Users\mauri\Documents\ING\"; 
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    //txtBody.Text += Path.GetFileName(fileName) + Environment.NewLine;
                    FileLists.Add(Path.GetFullPath(fileName));
                }
            }
        }
    }
}
