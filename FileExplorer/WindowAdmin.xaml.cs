using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Diagnostics;
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
using Wpf.Ui.Common;

namespace FileExplorer
{
    /// <summary>
    /// Lógica de interacción para WindowAdmin.xaml
    /// </summary>
    public partial class WindowAdmin : Window
    {
        public ObservableCollection<ItemData> Items { get; set; }
        public ObservableCollection<ItemDataUsers> ItemsUsers { get; set; }

        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/mauri/source/repos/f//FileExplorer/attFiles.accdb;";
        List<string> clients = new List<string>();
        Dictionary<string, string> usuarios = new Dictionary<string, string>();

        public WindowAdmin()
        {
            InitializeComponent();
            Items = new ObservableCollection<ItemData>();
            ItemsUsers = new ObservableCollection<ItemDataUsers>();
            addItemsClientes();
            addItemUsers();
            foreach (var clien in clients)
            {
                Items.Add(new ItemData { Name = clien, EditCommand = new RelayCommand(EditItem), DeleteCommand = new RelayCommand(DeleteItem) });
            }

            foreach (var user in usuarios)
            {
                ItemsUsers.Add(new ItemDataUsers { Name = user.Key, Password = user.Value, EditCommand = new RelayCommand(EditItem), DeleteCommand = new RelayCommand(DeleteItem) });
            }
            dynamicTable.ItemsSource = Items;
            dynamicTableUsers.ItemsSource = ItemsUsers;
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
                    Debug.WriteLine(reader.GetValue(0));
                    clients.Add(reader.GetValue(0).ToString());
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
                    Debug.WriteLine(reader.GetValue(0));
                    usuarios.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
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

        private void DeleteItem(object item)
        {
            // Handle delete command here
            ItemData selectedItem = (ItemData)item;
            MessageBox.Show("Delete " + selectedItem.Name);
            Items.Remove(selectedItem);
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
    }
}
