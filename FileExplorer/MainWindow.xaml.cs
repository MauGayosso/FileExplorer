
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Runtime.Remoting.Messaging;
using System.Windows.Input;
using System.Diagnostics;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Net;
using System.Net.Mail;
using MessageBox = System.Windows.MessageBox;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Media;
using CheckBox = System.Windows.Controls.CheckBox;
using Assimp;
using Assimp.Configs;
using Microsoft.Extensions.Options;
using Assimp.Unmanaged;
using Microsoft.Scripting.Hosting;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        private  string MODELPATH = Node.selectedBytes ;
        public MainWindow()
        {
            InitializeComponent();
            //load up last file used
            LoadPathFile();
            DataContext = this;
            instace = this;
        
            ModelVisual3D device3d = new ModelVisual3D();
           // device3d.Content = Display3d()
        }
        public void path3d(String MODEL_PATH) {
            Model3D device = null;
            if (MODEL_PATH  == ".stl") {
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
                catch(Exception err)
                {
                    System.Windows.MessageBox.Show("There has been an error loading the previously parsed directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (Node.selectedBytes is null)
            {
                System.Windows.MessageBox.Show("Por favor selecciona un documento", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (Path.GetExtension(Node.selectedBytes).Equals(".stl", StringComparison.OrdinalIgnoreCase))
                {
                    Window3D win = new Window3D();
                    win.Display3d(Node.selectedBytes);
                    win.Show();
                }
                else if (Path.GetExtension(Node.selectedBytes).Equals(".step", StringComparison.OrdinalIgnoreCase)) {
                    var stepPath = Node.selectedBytes;
                    WindowSTEP win = new WindowSTEP();
                    win.LoadStepFile(stepPath);
                    win.Show();
                    //var stlPath = "C:/Users/mauri/OneDrive/Escritorio/"+Path.GetFileName(Node.selectedBytes)+".stl";
                    var stlPath = "C:/Users/mauri/OneDrive/Escritorio/";

                    /*ScriptEngine engine = Python.CreateEngine();
                    dynamic script = engine.ExecuteFile("");

                    dynamic meshLabServer = script.MeshLabServer();

                    dynamic scriptText = script.Format(@"mls = 
                        meshlabserver.MeshLabServer()
                        msl.loadNew()
                        msl.open('{0}')
                        msl.saveCurrentMesh('{1}')
                        msl.close()",stepPath,stlPath);
                    engine.Execute(scriptText,meshLabServer);*/

                    
                }
                else
                {
                    Debug.WriteLine("NODE PATH SELECT : " + Node.selectedBytes);
                    System.Diagnostics.Process.Start("explorer.exe", Node.selectedBytes);
                }
            }
            
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
            this.Dispatcher.Invoke(DispatcherPriority.Background, ((Action)(() => {
                parseMsg.Visibility = Visibility.Hidden;
                firstNode = node;
                //firstNode.isChecked = false;
                UpdateCounts();
                fileDisplay.ItemsSource = treeCtx;
                //System.Windows.MessageBox.Show(Properties.Resources.parseSuccess);
            })));
        }

        //directory changed -> parse the new directory
        private void dirDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            ParseNewDir();
        }

        //checkbox clicked update counts display
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
            //save current directory to text file on window close
            try
            {
                System.IO.File.WriteAllText(@"path.txt", parseDir);
            }
            catch(Exception err)
            {
                System.Windows.MessageBox.Show("There has been an error saving the current directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string pathF = Node.selectedBytes;
            Debug.WriteLine("NODE CLICKED CONTEXT MENU :" + pathF);
        }

        private void SendConfirmation()
        {
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            mailItem.Subject = "Hello from test mail";
            mailItem.Body = "Example of mail";
            mailItem.To = "maugayosso1405@gmail.com";

            mailItem.Send();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(mailItem);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(outlookApp);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SendConfirmation();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchItem = txtSearch.Text;
            Debug.WriteLine("SEARCH TEXT : " + searchItem);
            SearchItemTreeView(fileDisplay.Items, searchItem);
            


        }

        private void SearchItemTreeView(ItemCollection nodes, string searchItem)
        {
            foreach (var node in nodes)
            {
                Node item = (Node)node;
                bool isMatCh = item.name.ToString().ToLower().Contains(searchItem);
                if (isMatCh)
                {

                }
               // SearchItemTreeView(item, searchItem);
            }
        }          
    }
}
