using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Modul6
{
    public partial class MainWindow : Window
    {
        private int selectedTransactionID = -1;
        private Data data = new Data();

        public MainWindow()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                data.openConnection();

                decimal amount = decimal.Parse(txtСумма.Text);
                string description = txtОписание.Text;
                string transactionType = (cmbТипТранзакции.SelectedItem as ComboBoxItem).Content.ToString();

                if (selectedTransactionID == -1)
                {
                    string query = "INSERT INTO modyl6_db (Date_of, Descr_of, Summ, Doxod) VALUES (@Date_of, @Descr_of, @Summ, @Doxod)";
                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@Date_of", DateTime.Now);
                        command.Parameters.AddWithValue("@Descr_of", description);
                        command.Parameters.AddWithValue("@Summ", amount);
                        command.Parameters.AddWithValue("@Doxod", transactionType == "Доход" ? 1 : 0);

                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Update an existing transaction
                    string query = "UPDATE modyl6_db SET Date_of = @Date_of, Descr_of = @Descr_of, Summ = @Summ, Doxod = @Doxod WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        // Set parameters
                        command.Parameters.AddWithValue("@Id", selectedTransactionID);
                        command.Parameters.AddWithValue("@Date_of", DateTime.Now);
                        command.Parameters.AddWithValue("@Descr_of", description);
                        command.Parameters.AddWithValue("@Summ", amount);
                        command.Parameters.AddWithValue("@Doxod", transactionType == "Доход" ? 1 : 0);

                        // Execute query
                        command.ExecuteNonQuery();
                    }
                }

                LoadTransactions();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                data.closeConnection();
            }
        }

        private void txtСумма_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!decimal.TryParse(txtСумма.Text + e.Text, out _))
            {
                e.Handled = true; // Отменить ввод неправильных символов
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                int transactionID = Convert.ToInt32(selectedRow["Id"]);

                try
                {
                    data.openConnection();

                    string query = "DELETE FROM modyl6_db WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@Id", transactionID);
                        command.ExecuteNonQuery();
                    }

                    LoadTransactions();
                    ResetForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
                finally
                {
                    data.closeConnection();
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }
        private void ClearDatabase(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите очистить базу данных?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    data.openConnection();

                    string query = "DELETE FROM modyl6_db";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.ExecuteNonQuery();
                    }

                    LoadTransactions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
                finally
                {
                    data.closeConnection();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                selectedTransactionID = Convert.ToInt32(selectedRow["Id"]);
                txtСумма.Text = selectedRow["Summ"].ToString();
                txtОписание.Text = selectedRow["Descr_of"].ToString();
                cmbТипТранзакции.Text = selectedRow["Doxod"].ToString() == "1" ? "Доход" : "Расход";
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.");
            }
        }

        private void LoadTransactions()
        {
            try
            {
                data.openConnection();

                string query = "SELECT * FROM modyl6_db";

                using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    listView.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                data.closeConnection();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Сумма" || textBox.Text == "Описание")
            {
                textBox.Text = "";
            }
        }

        private void ResetForm()
        {
            selectedTransactionID = -1;
            txtСумма.Text = "Сумма";
            txtОписание.Text = "Описание";
            cmbТипТранзакции.SelectedIndex = -1;
        }
    }
}
