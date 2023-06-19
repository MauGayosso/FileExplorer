
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Windows.Devices.PointOfService;
using Material = System.Windows.Media.Media3D.Material;
using MessageBox = System.Windows.MessageBox;
using Outlook = Microsoft.Office.Interop.Outlook;
using Path = System.IO.Path;
using Notifications.Wpf;
using Aspose.CAD.FileFormats.Cgm;
using System.Collections.Generic;
using Notifications.Wpf.Controls;
using Newtonsoft.Json.Linq;
using System.Data.OleDb;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowAyudasVisuales.xaml
    /// </summary>
    public partial class WindowAyudasVisuales : Window
    {
        public static System.Windows.Media.Color WindowGlassColor { get; }
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f//FileExplorer/attFiles.accdb;";
        List<string> titulos = new List<string>();
        List<string> mensajes = new List<string>();

        private delegate Node ParseDirDelegate();
        // Notifications
        private readonly NotificationManager notificationManager = new NotificationManager();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly List<NotificationContent> notifications = new List<NotificationContent>();

        //tree display source
        ObservableCollection<Node> treeCtx = new ObservableCollection<Node>();
        Node firstNode;

        //Dependancy properties for all labels
        public string parseDirCorte
        {
            get { return (string)GetValue(parseDirProp); }
            set { SetValue(parseDirProp, value); }
        }

        public static readonly DependencyProperty parseDirProp =
            DependencyProperty.Register("parseDirCorte", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public int folders
        {
            get { return (int)GetValue(foldersProp); }
            set { SetValue(foldersProp, value); }
        }

        public static readonly DependencyProperty foldersProp =
            DependencyProperty.Register("foldersCorte", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int files
        {
            get { return (int)GetValue(filesProp); }
            set { SetValue(filesProp, value); }
        }

        public static readonly DependencyProperty filesProp =
            DependencyProperty.Register("filesCorte", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int selectedFolders
        {
            get { return (int)GetValue(selectedFoldersProp); }
            set { SetValue(selectedFoldersProp, value); }
        }

        public static readonly DependencyProperty selectedFoldersProp =
            DependencyProperty.Register("selectedFoldersCorte", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int selectedFiles
        {
            get { return (int)GetValue(selectedFilesProp); }
            set { SetValue(selectedFilesProp, value); }
        }

        public static readonly DependencyProperty selectedFilesProp =
            DependencyProperty.Register("selectedFilesCorte", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public string sizeInBytes
        {
            get { return (string)GetValue(sizeInBytesProp); }
            set { SetValue(sizeInBytesProp, value); }
        }

        public static readonly DependencyProperty sizeInBytesProp =
            DependencyProperty.Register("sizeInBytesCorte", typeof(string), typeof(MainWindow), new PropertyMetadata((string)""));
        private string MODELPATH = Node.selectedBytes;
        //Dependancy properties for all labels
        public WindowAyudasVisuales()
        {
            InitializeComponent();
            LoadPathFile();
            DataContext = this;
            Brush titleBarBrush = new SolidColorBrush(WindowGlassColor);

            getMessages();
            foreach (var m in mensajes)
            {
                notifications.Add(new NotificationContent { Title = "Tip", Message = m, Type = NotificationType.Information });
            }

            // Configure and start the timer
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void getMessages()
        {
            var query = "SELECT * FROM messages";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.ExecuteNonQuery();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Debug.WriteLine(reader.GetValue(0));
                    mensajes.Add(reader.GetValue(0).ToString());
                }
                reader.Close();
                connection.Close();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Get a random notification from the list
            Random random = new Random();
            int index = random.Next(notifications.Count);
            NotificationContent notification = notifications[index];

            // Show the notification
            notificationManager.Show(notification, areaName: "WindowArea");
        }
        public void LoadPathFile()
        {
            //if saved path exists load else parse current dir
            /* if (File.Exists("./pathAVCorte.txt"))
                 try
                 {
                     using (StreamReader inputFile = new StreamReader("./pathAVCorte.txt", true))
                     {
                         parseDirCorte = inputFile.ReadLine();
                     }
                 }
                 catch (Exception err)
                 {
                     MessageBox.Show("There has been an error loading the previously parsed directory. " + err, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                 }
             else
             {
                 //Get full path of current directory
                 parseDirCorte = Path.GetFullPath(".");
             }*/
        }
        private void createFirstNode()
        {
            //initialize first node to hold all other nodes
            DirectoryInfo dirInfo = new DirectoryInfo(parseDirCorte);
            firstNode = new Node()
            {
                name = dirInfo.Name,
                fullPath = parseDirCorte,
                byteSize = 0,
                parent = null,
                iconLoc = Node.folderIcon,
                isFile = false,
            };
            ++Node.folders;
            //add first node to display         
            treeCtx = new ObservableCollection<Node>()
            {
                firstNode
            };
        }

        //update all dependacy properties to current static values in Node class
        private void UpdateCounts()
        {
            files = Node.files;
            folders = Node.folders;
            selectedFolders = Node.selectedFolders;
            selectedFiles = Node.selectedFiles;
            sizeInBytes = Node.selectedBytes;
        }

        public void viewTree_NodeChecked(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            DirectoryInfo _NewPath = (DirectoryInfo)newSelected.Tag;
            if (_NewPath != null && !string.IsNullOrWhiteSpace(_NewPath.FullName))
            {
                Debug.WriteLine("NEW PATH : " + _NewPath.FullName);
            }
        }
        public void viewTree_PreviewMouseRightClickDown(object sender, MouseButtonEventArgs e)
        {
            var carpetaOnly = Path.GetDirectoryName(Node.selectedBytes);
            Debug.WriteLine("1: " + carpetaOnly);
            parseDirCorte = carpetaOnly;
            //LoadPathFile();
            ParseNewDir();
            // createFirstNode();
        }

        public void viewTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        private void ParseNewDir()
        {
            //resets all counts and displays to 0
            Node.resetCounts();
            UpdateCounts();

            //create first node, empty tree display, display parsing msg
            createFirstNode();
            fileDisplay.ItemsSource = null;
            parseMsg.Visibility = Visibility.Visible;

            //recursively parse directory asynchronously 
            ParseDirDelegate parseDelegate = new ParseDirDelegate(firstNode.DirParse);
            parseDelegate.BeginInvoke(theCallback, this);
        }

        public void theCallback(IAsyncResult theResults)
        {
            AsyncResult result = (AsyncResult)theResults;
            ParseDirDelegate parseDelegate = (ParseDirDelegate)result.AsyncDelegate;
            Node node = parseDelegate.EndInvoke(theResults);
            //Back to the GUI thread to update tree display and counts with newly parsed directory
            //hide parsing msg and display complete msg
            this.Dispatcher.Invoke(DispatcherPriority.Background, ((Action)(() =>
            {
                parseMsg.Visibility = Visibility.Hidden;
                firstNode = node;
                //firstNode.isChecked = false;
                UpdateCounts();
                fileDisplay.ItemsSource = treeCtx;
            })));
        }
        private void dirDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            ParseNewDir();
        }

        private void chk_clicked(object sender, RoutedEventArgs e)
        {
            UpdateCounts();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //display folder dialog so user can select directory to parse
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    parseDirCorte = fbd.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                System.IO.File.WriteAllText(@"pathAVCorte.txt", parseDirCorte);
            }
            catch (Exception err)
            {
                //System.Windows.MessageBox.Show("There has been an error saving the current directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string pathF = Node.selectedBytes;
            timer.Stop();

        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //twSearched.Items.Clear();
            //string searchItem = txtSearch.Text;
            // SearchItemTreeView(searchItem);
        }

        private void SearchItemTreeView(string searchItem)
        {
            string[] files = Directory.GetFiles("C:/Users/mauri/Documents/ING/", searchItem, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                // return files[0]
                foreach (string file in files)
                {
                    //  twSearched.Items.Add(file);
                }
            }
            else
            {

            }
        }
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemsPanel.Visibility == Visibility.Visible)
            {
                MenuItemsPanel.Visibility = Visibility.Hidden;
            }
            else if (MenuItemsPanel.Visibility == Visibility.Hidden)
            {
                MenuItemsPanel.Visibility = Visibility.Visible;
            }

        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            //MainWindow winMain = new MainWindow();
            //winMain.Close();
            timer.Stop();
            WindowOptionsAV win = new WindowOptionsAV();
            win.Show();
            Close();
        }

        private void ExitClick(object sender, RoutedEventArgs e)

        {
            timer.Stop();
            Close();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (Node.selectedBytes == null)
            {
                System.Windows.MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {

                if (Path.GetExtension(Node.selectedBytes).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var pathPdf = Path.GetFullPath(Node.selectedBytes);
                    //extract_Rev();
                    //Process.Start(pathPdf);
                    Uri pdfUri = new Uri(pathPdf);
                    wb.Source = pdfUri;
                }
                else if (Path.GetExtension(Node.selectedBytes).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var pathExcel = Path.GetFullPath(Node.selectedBytes);
                    Process.Start(pathExcel);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
        private void folder_click(object sender, RoutedEventArgs e)
        {
            if (Node.selectedBytes == null)
            {
                System.Windows.MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Process.Start("explorer.exe", Path.GetDirectoryName(Node.selectedBytes));
            }
        }

        private void reloadTreeView_click(object sender, RoutedEventArgs e)
        {
            LoadPathFile();
            ParseNewDir();
        }

        private void InicioClick(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            WindowLogin win = new WindowLogin();
            WindowMail winMail = new WindowMail();
            // MainWindow winMain = new MainWindow();
            // winMain.Close();
            winMail.Close();
            win.Show();
            Close();
        }

    }
}


