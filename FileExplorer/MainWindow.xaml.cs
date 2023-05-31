
using HelixToolkit.Wpf;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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
using Material = System.Windows.Media.Media3D.Material;
using MessageBox = System.Windows.MessageBox;
using Outlook = Microsoft.Office.Interop.Outlook;
using Path = System.IO.Path;
//using SkiaSharp;
//using SkiaSharp.Views.WPF;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static System.Windows.Media.Color WindowGlassColor { get; }
        public static MainWindow instace;
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
            //load up last file used
            LoadPathFile();
            DataContext = this;
            instace = this;

            ModelVisual3D device3d = new ModelVisual3D();
            // device3d.Content = Display3d()
            Brush titleBarBrush = new SolidColorBrush(WindowGlassColor);

        }
        public void path3d(String MODEL_PATH)
        {
            Model3D device = null;
            if (MODEL_PATH == ".stl")
            {
                try
                {
                    //viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                    ModelImporter import = new ModelImporter();
                    device = import.Load(MODEL_PATH);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Exception Error : " + e.StackTrace);
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
                Debug.WriteLine("NEW PATH : " + _NewPath.FullName);
            }
        }
        public void viewTree_PreviewMouseRightClickDown(object sender, MouseButtonEventArgs e)
        {
            var carpetaOnly = Path.GetDirectoryName(Node.selectedBytes);
            Debug.WriteLine("1: " + carpetaOnly);
            parseDir = carpetaOnly;
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
                    parseDir = fbd.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                System.IO.File.WriteAllText(@"path.txt", parseDir);
            }
            catch (Exception err)
            {
                //System.Windows.MessageBox.Show("There has been an error saving the current directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            SearchItemTreeView(searchItem);
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
            if (Node.selectedBytes == null)
            {
                System.Windows.MessageBox.Show("Selecciona un archivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                if (Path.GetExtension(Node.selectedBytes).Equals(".stl", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        Model3D device = null;
                        Material material = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(91, 91, 92)));
                        ModelVisual3D device3d = new ModelVisual3D();
                        viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                        ModelImporter import = new ModelImporter();
                        viewPort3d.Children.Remove(device3d);
                        import.DefaultMaterial = material;
                        device = import.Load(Node.selectedBytes);
                        device3d.Content = device;
                        viewPort3d.Children.Add(device3d);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar modelo STL:" + ex.StackTrace, "Error", MessageBoxButton.OK);
                    }
                }
                else if (Path.GetExtension(Node.selectedBytes).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    grid3d.Children.Remove(viewPort3d);
                    grid3d.Children.Remove(txtTextBox);
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
                else if (Path.GetExtension(Node.selectedBytes).Equals(".plt", StringComparison.OrdinalIgnoreCase))
                {
                    grid3d.Children.Remove(viewPort3d);
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

        public void extract_Rev()
        {
            string pfdPath = Path.GetFullPath(Node.selectedBytes);
            string wordReference = "MATERIAL";

            StringBuilder textBuilder = new StringBuilder();

            // Open the PDF file
            using (PdfReader reader = new PdfReader(pfdPath))
            {
                // Iterate over each page of the PDF
                for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber++)
                {
                    // Extract text from the current page
                    string pageText = PdfTextExtractor.GetTextFromPage(reader, pageNumber);

                    // Check if the keyword exists in the extracted text
                    if (pageText.Contains(wordReference))
                    {
                        // Append the extracted text to the StringBuilder
                        textBuilder.Append(pageText);
                    }
                }
            }
            // Get the final extracted text
            string extractedText = textBuilder.ToString();
            txtTextBox.Text = extractedText;
            Debug.WriteLine("Revision : " + extractedText);
        }
    }
}
