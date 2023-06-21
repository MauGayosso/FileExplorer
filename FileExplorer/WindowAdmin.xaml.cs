using DocumentFormat.OpenXml.Office.Word;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.UI.Xaml.Controls.Primitives;
using Wpf.Ui.Common;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowAdmin.xaml
    /// </summary>
    public partial class WindowAdmin : Window
    {
        public ObservableCollection<ItemData> Items { get; set; }
        public ObservableCollection<ItemDataUsers> ItemsUsers { get; set; }
        public ObservableCollection<ItemDataTip> ItemTips { get; set; }

        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f//FileExplorer/attFiles.accdb;";
        Dictionary<string, string> clients = new Dictionary<string, string>();
        Dictionary<string, string> usuarios = new Dictionary<string, string>();
        Dictionary<string, string> tips = new Dictionary<string, string>();

        public WindowAdmin()
        {
            InitializeComponent();
            Items = new ObservableCollection<ItemData>();
            ItemsUsers = new ObservableCollection<ItemDataUsers>();
            ItemTips = new ObservableCollection<ItemDataTip>();
            // Load data
            addItemsClientes();
            addItemUsers();
            addItemsTips();
            // Load Grid with info
            addToGridUsers();
            addToGridClient();
            addToGridTips();
        }

        private void addToGridTips()
        {
            foreach (var tip in tips)
            {
                Debug.WriteLine(tip);
                ItemTips.Add(new ItemDataTip { Message = tip.Key, Category = tip.Value, EditCommand = new RelayCommand(EditItemTip), DeleteCommand = new RelayCommand(DeleteItemTip) });
            }
            dynamicTableTips.ItemsSource = ItemTips;
        }
        private void addToGridUsers()
        {
            foreach (var user in usuarios)
            {
                ItemsUsers.Add(new ItemDataUsers { Name = user.Key, Password = user.Value, EditCommand = new RelayCommand(EditItemTip), DeleteCommand = new RelayCommand(DeleteItemUsers) });
            }

            dynamicTableUsers.ItemsSource = ItemsUsers;
        }
        private void addToGridClient()
        {
            foreach (var clien in clients)
            {
                Items.Add(new ItemData { Name = clien.Key, PathClient = clien.Value, EditCommand = new RelayCommand(EditItem), DeleteCommand = new RelayCommand(DeleteItem) });
            }
            dynamicTable.ItemsSource = Items;
        }
        private void addItemsClientes()
        {
            var query = "SELECT * FROM clientes";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.ExecuteNonQuery();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    clients.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
                }
                reader.Close();
                connection.Close();
            }
        }

        private void addItemUsers()
        {
            var query = "SELECT * FROM usuarios";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);
                command.ExecuteNonQuery();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    usuarios.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
                }
                reader.Close();
                connection.Close();
            }
        }

        private void addItemsTips()
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
                    tips.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
                }
                reader.Close();
                connection.Close();
            }
        }


        private void EditItem(object item)
        {
            // Handle edit command here
            ItemData selectedItem = (ItemData)item;
            MessageBox.Show("Edit " + selectedItem.Name);
            string query = "UPDATE clientes SET client_name = ?  WHERE client_name = ?";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clientValue", selectedItem.Name);
                    command.Parameters.AddWithValue("@clientValu", selectedItem.Name);


                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Actualizado correctamente", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar valor", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

        }

        // EDIT BUTTON TIP 
        private void EditItemTip()
        {

        }

        //EDIT BUTTON USER

        private void EditItemUser()
        {

        }

        // DELETE BUTTON TO CLIENTS
        private void DeleteItem(object item)
        {
            // Handle delete command here
            ItemData selectedItem = (ItemData)item;
            MessageBoxResult result = MessageBox.Show("¿Estas seguro de eliminar el cliente : " + selectedItem.Name + "?", "Eliminar", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                string query = "DELETE FROM clientes WHERE id_usuario = ?";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_usuario", selectedItem.Name);
                        int rows = command.ExecuteNonQuery();
                    }
                }
                clients.Clear();
                Items.Clear();
                dynamicTable.ItemsSource = null;
                addItemsClientes();
                addToGridClient();
            }
        }


        // DELETE BUTTON TO USERS
        private void DeleteItemUsers(object itemUsers)
        {
            // Handle delete command here
            ItemDataUsers selectedItem = (ItemDataUsers)itemUsers;
            MessageBoxResult result = MessageBox.Show("¿Estas seguro de eliminar el cliente : " + selectedItem.Name + "?", "Eliminar", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                string query = "DELETE FROM usuarios WHERE id_usuario = ?";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_usuario", selectedItem.Name);
                        int rows = command.ExecuteNonQuery();
                    }
                }
                usuarios.Clear();
                ItemsUsers.Clear();
                dynamicTableUsers.ItemsSource = null;
                addItemUsers();
                addToGridUsers();
            }
        }

        // DELETE BUTTON TO TIPS

        private void DeleteItemTip()
        {

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
            WindowOptions win = new WindowOptions();
            win.Show();
            Close();
        }

        private void ExitClick(object sender, RoutedEventArgs e)

        {
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //BUTTON RELOAD CLIENTS PAGE
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            clients.Clear();
            Items.Clear();
            dynamicTable.ItemsSource = null;
            addItemsClientes();
            addToGridClient();
        }

        //BUTTON RELOAD USERS PAGE
        private void btnReloadUsers_Click_1(object sender, RoutedEventArgs e)
        {
            usuarios.Clear();
            ItemsUsers.Clear();
            dynamicTableUsers.ItemsSource = null;
            addItemUsers();
            addToGridUsers();
        }

        private void btnSelectFolderCliente_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the selected folder path from the dialog
                    string folderPath = folderBrowserDialog.SelectedPath;
                    lblFolderCliente.Content = folderPath;
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lblFolderCliente.Content == null)
            {
                MessageBox.Show("Selecciona una carpeta", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (txtNameInsertClient.Text == null)
            {
                MessageBox.Show("Nombre faltante", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                var query = "insert into clientes (id_usuario,path_cliente) values (@id_usuario,@path_cliente)";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_usuario", txtNameInsertClient.Text);
                        command.Parameters.AddWithValue("@path_cliente", lblFolderCliente.Content);
                        command.ExecuteNonQuery();
                    }
                }
                clients.Clear();
                Items.Clear();
                dynamicTable.ItemsSource = null;
                addItemsClientes();
                addToGridClient();
            }

        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            if (canvaCliente.Visibility == Visibility.Hidden)
            {
                btnAddClient.BorderBrush = Brushes.Red;
                btnAddClient.BorderThickness = new Thickness(2);
                canvaCliente.Visibility = Visibility.Visible;
            }
            else
            {
                btnAddClient.BorderBrush = Brushes.Green;
                btnAddClient.BorderThickness = new Thickness(2);
                canvaCliente.Visibility = Visibility.Hidden;
            }
        }

        private void dynamicTableTips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
