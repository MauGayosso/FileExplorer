using System.Data.OleDb;
using HelixToolkit.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Material = System.Windows.Media.Media3D.Material;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using Exception = System.Exception;
using Window = System.Windows.Window;
using Brush = System.Windows.Media.Brush;
using DuEDrawingControl;

namespace FileExplorer

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// The variable Node.selectedBytes is to get the path from the checkbox selected ===> It can be modified but you need to do it in the file Node.cs
    /// This project was made by https://github.com/MauGayosso - period 05/09/23 to 08/18/2023 for the enterprise Maquinados Industriales
    public partial class MainWindow : Window
    {
        //conection to database in access
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f/FileExplorer/MI_DB/attFiles.accdb");
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f/FileExplorer/MI_DB/attFiles.accdb;";
        public static Color WindowGlassColor { get; }

        //Reference to a EDrawing to WPF
        private EDrawingWPFControl eDrawingView;

        // Parse Dir
        private delegate Node ParseDirDelegate();

        //tree display source
        ObservableCollection<Node> treeCtx = new ObservableCollection<Node>();
        Node firstNode;

        //Dependancy properties for all labels
        public string parseDir
        {
            get { return (string)GetValue(parseDirProp); }
            set { SetValue(parseDirProp, value); }
        }

        // Register parseDirProp -> Needs to be modified if you use this code in another window
        public static readonly DependencyProperty parseDirProp =
            DependencyProperty.Register("parseDir", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        // Number of folders
        public int folders
        {
            get { return (int)GetValue(foldersProp); }
            set { SetValue(foldersProp, value); }
        }

        // Register FoldersProp -> Needs to be modified if you use this code in another window
        public static readonly DependencyProperty foldersProp =
            DependencyProperty.Register("folders", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int files
        {
            get { return (int)GetValue(filesProp); }
            set { SetValue(filesProp, value); }
        }

        // Register FilesProp -> Needs to be modified if you use this code in another window
        public static readonly DependencyProperty filesProp =
            DependencyProperty.Register("files", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int selectedFolders
        {
            get { return (int)GetValue(selectedFoldersProp); }
            set { SetValue(selectedFoldersProp, value); }
        }

        // Register selectedFoldersProp -> Needs to be modified if you use this code in another window
        public static readonly DependencyProperty selectedFoldersProp =
            DependencyProperty.Register("selectedFolders", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int selectedFiles
        {
            get { return (int)GetValue(selectedFilesProp); }
            set { SetValue(selectedFilesProp, value); }
        }

        // Register selectedFilesProp -> Needs to be modified if you use this code in another window
        public static readonly DependencyProperty selectedFilesProp =
            DependencyProperty.Register("selectedFiles", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public string sizeInBytes
        {
            get { return (string)GetValue(sizeInBytesProp); }
            set { SetValue(sizeInBytesProp, value); }
        }

        // Register sizeInBytesProp -> Needs to be modified if you use this code in another window
        public static readonly DependencyProperty sizeInBytesProp =
            DependencyProperty.Register("sizeInBytes", typeof(string), typeof(MainWindow), new PropertyMetadata((string)""));
        public MainWindow()
        {
            InitializeComponent();
            LoadPathFile();
            DataContext = this;
            Brush titleBarBrush = new SolidColorBrush(WindowGlassColor);
            eDrawingView = edrawingControl;

        }

        public void LoadPathFile()
        {
            //if saved path exists load else parse current dir
            if (File.Exists("./path.txt"))
                try
                {
                    using (StreamReader inputFile = new StreamReader("path.txt", true))
                    {
                        parseDir = inputFile.ReadLine();
                    }
                }
                catch (Exception err)
                {
                    System.Windows.MessageBox.Show("There has been an error loading the previously parsed directory. " + err, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            else
            {
                //Get full path of current directory
                parseDir = Path.GetFullPath(".");
            }
        }
        private void createFirstNode()
        {
            //initialize first node to hold all other nodes
            if (parseDir is null)
            {

            }
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(parseDir);

                firstNode = new Node()
                {
                    name = dirInfo.Name,
                    fullPath = parseDir,
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

            }
        }
        public void viewTree_PreviewMouseRightClickDown(object sender, MouseButtonEventArgs e)
        {
            var carpetaOnly = Path.GetDirectoryName(Node.selectedBytes);
            parseDir = carpetaOnly;

            ParseNewDir();

        }

        //It can be deleted --- Make sure to delete from .xaml
        public void viewTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        public void ParseNewDir()
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

            this.Dispatcher.Invoke(DispatcherPriority.Background, ((System.Action)(() =>
            {
                parseMsg.Visibility = Visibility.Hidden;
                firstNode = node;
                UpdateCounts();
                fileDisplay.ItemsSource = treeCtx;
            })));
        }


        private void dirDisplay_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ParseNewDir();
        }

        private void chk_clicked(object sender, RoutedEventArgs e)
        {
            listAtts.Items.Clear();
            ContentPath();

            UpdateCounts();
            attributesFiles(Node.selectedBytes);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //display folder dialog so user can select directory to parse
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    parseDir = fbd.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                File.WriteAllText(@"path.txt", parseDir);
            }
            catch (Exception err)
            {
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string pathF = Node.selectedBytes;
        }

        private void SendConfirmation()
        {
            WindowMail win = new WindowMail();
            win.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SendConfirmation();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            twSearched.Items.Clear();
            string searchItem = txtSearch.Text;
            SearchBox(searchItem);
        }
        private void SearchItemTreeView(string searchItem)
        {
            string[] files = Directory.GetFiles("C:/Users/mauri/Documents/ING/", searchItem, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                // return files[0]
                foreach (string file in files)
                {
                    twSearched.Items.Add(file);
                }
            }
            else
            {

            }
        }
        private void SearchBox(string searchItem)
        {
            string[] files = Directory.GetFiles("C:/Users/mauri/Documents/ING/", searchItem + ".*", SearchOption.AllDirectories);
            foreach (string file in files)
            {

                twSearched.Items.Add(Path.GetFileName(file));
            }

            // Recursively search for subdirectories
            string[] subdirectories = Directory.GetDirectories("C:/Users/mauri/Documents/ING/");
            foreach (string subdirectory in subdirectories)
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
            WindowClientsMenu win = new WindowClientsMenu();
            win.Show();
            Close();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            WindowLogin win = new WindowLogin();
            win.Show();
            Close();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            toolbarOption(Node.selectedBytes);
        }

        private void toolbarOption(string path)
        {
            if (path == null)
            {
                MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                if (Path.GetExtension(path).Equals(".PDF", StringComparison.OrdinalIgnoreCase))
                {
                    edrawingControl.Visibility = Visibility.Hidden;
                    wb.Visibility = Visibility.Visible;
                    var pathPdf = Path.GetFullPath(path);
                    Uri pdfUri = new Uri(pathPdf);
                    wb.Source = pdfUri;
                }
                else if (Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var pathExcel = Path.GetFullPath(path);
                    Process.Start(pathExcel);
                }
                else if (Path.GetExtension(path).Equals(".SLDPRT", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".dxf", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".STEP", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".STL", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".OBJ", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".SDLASM", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".dwg", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".stp", StringComparison.OrdinalIgnoreCase)|| Path.GetExtension(path).Equals(".SLDDRW", StringComparison.OrdinalIgnoreCase))
                {
                    //grid3d.Children.Remove(wb);
                    wb.Visibility = Visibility.Hidden;
                    edrawingControl.Visibility = Visibility.Visible;
                    eDrawingView = edrawingControl;
                    var testModel = Path.GetFullPath(Node.selectedBytes);
                    eDrawingView.EDrawingHost.OpenDoc(testModel, false, false, false);
                }
            }
        }

        private void EDrawingHost_OnControlLoaded(dynamic obj)
        {
            eDrawingView = edrawingControl;
            var testModel = Path.GetFullPath(Node.selectedBytes);
            eDrawingView.EDrawingHost.OpenDoc(testModel, false, false, false);

        }

        private void SaveInfoFile_Click(object sender, RoutedEventArgs e)
        {
            if (Node.selectedBytes != null)
            {
                WindowAtts win = new WindowAtts();
                win.pathPass = Node.selectedBytes;
                win.loadName(Node.selectedBytes);
                win.Show();
            }
            else
            {
                MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Exclamation);
            }
        }

        private void EditInfoFile_Click(object sender, RoutedEventArgs e)
        {
            if (Node.selectedBytes != null)
            {
                WindowEdit win = new WindowEdit();
                win.loadName(Node.selectedBytes);
                win.atts(Node.selectedBytes);
                win.pathPass = Node.selectedBytes;
                win.Show();
            }
            else
            {
                MessageBox.Show("Selecciona una archivo para editar", "Error");
            }

        }

        private void folder_click(object sender, RoutedEventArgs e)
        {
            if (Node.selectedBytes == null)
            {
                MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void backTreeView_click(object sender, RoutedEventArgs e)
        {
            var carpetaOnly = Path.GetDirectoryName(Path.GetDirectoryName(Node.selectedBytes));
            if (carpetaOnly != null)
            {
                parseDir = carpetaOnly;
                ParseNewDir();
            }
            else
            {
                MessageBox.Show("No existen mas carpetas");
            }
        }

        public void attributesFiles(string id)
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
                    listAtts.Items.Add("Revision: " + reader.GetValue(1));
                    listAtts.Items.Add("Materia Prima: " + reader.GetValue(2));
                }
                reader.Close();
                connection.Close();
            }
        }

        private void ContentPath()
        {
            string parentPath = Directory.GetParent(Node.selectedBytes)?.FullName;
            Debug.WriteLine("pp: " + Path.GetDirectoryName(parentPath));
            listFilesNode.Items.Clear();
            var pathF = Path.GetDirectoryName(Node.selectedBytes);

            var path = Directory.GetDirectories(parentPath, "*", SearchOption.TopDirectoryOnly);

            string[] directories = Directory.GetDirectories(pathF);
            foreach (string directory in directories)
            {

                listFilesNode.Items.Add("Carpeta: " + directory.Replace(Path.GetDirectoryName(directory) + Path.DirectorySeparatorChar, ""));
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var path = twSearched.SelectedItem.ToString();
            if (path == null)
            {
                MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                if (Path.GetExtension(path).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    grid3d.Children.Remove(edrawingControl);
                    var pathPdf = Path.GetFullPath(path);
                    Uri pdfUri = new Uri(pathPdf);
                    wb.Source = pdfUri;
                }
                else if (Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var pathExcel = Path.GetFullPath(path);
                    Process.Start(pathExcel);
                }
                else if (Path.GetExtension(path).Equals(".SLDPRT", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".dxf", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".STEP", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".STL", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".OBJ", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".SDLASM", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".dwg", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(path).Equals(".stp", StringComparison.OrdinalIgnoreCase))
                {
                    eDrawingView = edrawingControl;
                    var testModel = Path.GetFullPath(path);
                    eDrawingView.EDrawingHost.OpenDoc(testModel, false, false, false);
                }
            }

        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var path = twSearched.SelectedItem.ToString();
            if (path == null)
            {
                MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Process.Start("explorer.exe", Path.GetDirectoryName(path));
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (twSearched.SelectedItem != null)
            {
                WindowAtts win = new WindowAtts();
                win.loadName(twSearched.SelectedItem.ToString());
                win.pathPass = twSearched.SelectedItem.ToString();
                win.Show();
            }
            else
            {
                MessageBox.Show("Selecciona un archivo", "Error");
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (twSearched.SelectedItem.ToString() != null)
            {
                WindowEdit win = new WindowEdit();
                win.loadName(twSearched.SelectedItem.ToString());
                win.atts(twSearched.SelectedItem.ToString());
                win.pathPass = twSearched.SelectedItem.ToString();
                win.Show();
            }
            else
            {
                MessageBox.Show("Selecciona una archivo para editar", "Error");
            }
        }

        private void edrawingControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            eDrawingView.Markup.ViewOperator_Set(EMVMarkupOperators.eMVOperatorMeasure);
        }
    }
}
