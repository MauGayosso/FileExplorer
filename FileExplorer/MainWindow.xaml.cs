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
    public partial class MainWindow : Window
    {
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/attFiles.accdb");
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/FileExplorer/FileExplorer/FileExplorer/MI_DB/attFiles.accdb;";
        public static Color WindowGlassColor { get; }

        private DuEDrawingControl.EDrawingWPFControl edrawing;
        private EDrawingView edrawingView;

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

        public static readonly DependencyProperty parseDirProp =
            DependencyProperty.Register("parseDir", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public int folders
        {
            get { return (int)GetValue(foldersProp); }
            set { SetValue(foldersProp, value); }
        }

        public static readonly DependencyProperty foldersProp =
            DependencyProperty.Register("folders", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int files
        {
            get { return (int)GetValue(filesProp); }
            set { SetValue(filesProp, value); }
        }

        public static readonly DependencyProperty filesProp =
            DependencyProperty.Register("files", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int selectedFolders
        {
            get { return (int)GetValue(selectedFoldersProp); }
            set { SetValue(selectedFoldersProp, value); }
        }

        public static readonly DependencyProperty selectedFoldersProp =
            DependencyProperty.Register("selectedFolders", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public int selectedFiles
        {
            get { return (int)GetValue(selectedFilesProp); }
            set { SetValue(selectedFilesProp, value); }
        }

        public static readonly DependencyProperty selectedFilesProp =
            DependencyProperty.Register("selectedFiles", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public string sizeInBytes
        {
            get { return (string)GetValue(sizeInBytesProp); }
            set { SetValue(sizeInBytesProp, value); }
        }

        public static readonly DependencyProperty sizeInBytesProp =
            DependencyProperty.Register("sizeInBytes", typeof(string), typeof(MainWindow), new PropertyMetadata((string)""));
        private string MODELPATH = Node.selectedBytes;
        public MainWindow()
        {
            InitializeComponent();
            LoadPathFile();
            DataContext = this;
            ModelVisual3D device3d = new ModelVisual3D();
            Brush titleBarBrush = new SolidColorBrush(WindowGlassColor);
        }
        public void path3d(String MODEL_PATH)
        {
            Model3D device = null;
            if (MODEL_PATH == ".stl")
            {
                try
                {
                    ModelImporter import = new ModelImporter();
                    device = import.Load(MODEL_PATH);
                }
                catch (System.Exception e)
                {
                    MessageBox.Show("Exception Error : " + e.StackTrace);
                }

            }
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
            //Back to the GUI thread to update tree display and counts with newly parsed directory
            //hide parsing msg and display complete msg
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

                twSearched.Items.Add(file);
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
                if (Path.GetExtension(path).Equals(".obj", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        grid3d.Children.Remove(wb);
                        grid3d.Children.Remove(previewImage);
                        grid3d.Children.Remove(txtTextBox);
                        Model3D device = null;
                        Material material = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Color.FromRgb(91, 91, 92)));
                        ModelVisual3D device3d = new ModelVisual3D();
                        viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                        ModelImporter import = new ModelImporter();
                        viewPort3d.Children.Remove(device3d);
                        import.DefaultMaterial = material;
                        device = import.Load(path);
                        device3d.Content = device;
                        viewPort3d.Children.Add(device3d);

                        var readerObj = new ObjReader();

                        var model3D = readerObj.Read(Node.selectedBytes);
                        var modelVisual3D = new ModelVisual3D();
                        viewPort3d.Children.Add(modelVisual3D); ;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar modelo:" + ex.StackTrace, "Error", MessageBoxButton.OK);
                    }
                }
                else if (Path.GetExtension(path).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    grid3d.Children.Remove(viewPort3d);
                    grid3d.Children.Remove(txtTextBox);
                    var pathPdf = Path.GetFullPath(path);
                    Uri pdfUri = new Uri(pathPdf);
                    wb.Source = pdfUri;
                }
                else if (Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var pathExcel = Path.GetFullPath(path);
                    Process.Start(pathExcel);
                }
                else if (Path.GetExtension(path).Equals(".plt", StringComparison.OrdinalIgnoreCase))
                {
                    grid3d.Children.Remove(viewPort3d);
                    grid3d.Children.Remove(txtTextBox);


                }
                else if (Path.GetExtension(path).Equals(".sldprt", StringComparison.OrdinalIgnoreCase))
                {
                    grid3d.Children.Remove(viewPort3d);
                    grid3d.Children.Remove(txtTextBox);
                    grid3d.Children.Remove(wb);
                    var testModel = Node.selectedBytes;
                    edrawing.EDrawingHost.OpenDoc(testModel, false, false, false);
                }
            }
        }

        private void EDrawingHost_OnControlLoaded(dynamic obj)
        {
            edrawing.EDrawingHost.OpenDoc(Node.selectedBytes, false, false, false);
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
                MessageBox.Show("Seleecionar un archivo");
            }
            else
            {
                if (Path.GetExtension(path) == ".pdf")
                {
                    grid3d.Children.Remove(viewPort3d);
                    grid3d.Children.Remove(txtTextBox);
                    var pathPdf = Path.GetFullPath(path);
                    Uri pdfUri = new Uri(pathPdf);
                    wb.Source = pdfUri;
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
        private void measure_Click(object sender, EventArgs e)
        {
        }

    }
}
