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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DumbSql
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly private static string supplierTab = @"..\..\data\supplierTab.txt";
        readonly private static string detailTab = @"..\..\data\detailTab.txt";
        readonly private static string deliveryTab = @"..\..\data\deliveryTab.txt";

        internal void RefreshTable(string path, IAdd data, DataGrid grid)
        {
            using (var textTab = File.OpenText(path))
            {
                while (!textTab.EndOfStream)
                {
                    grid.Items.Add(data.Create(textTab.ReadLine().Split(';')));
                }
            }
        }
        internal void SaveTable(string path, ITex data, DataGrid grid)
        {
            using (var textTab = new StreamWriter(path))
            {
                foreach(var item in grid.Items)
                {
                    var obj = (ITex)item;
                    textTab.WriteLine(obj.toTabular());
                }
            }
        }
        internal void AddInTable(IAdd data, DataGrid grid, params string[] args)
        {
            grid.Items.Add(data.Create(args));
        }
        internal object FindInTable<T>(T data, DataGrid grid, Func<T, T, bool> foo) where T : class
        {
            foreach (var item in grid.Items)
            {
                var obj = item as T;
                if (obj != null && foo(obj, data))
                    return item;
            }
            return null; 
        }
        internal void SelectInTable<T>(T data, DataGrid grid, Func<T, T, bool> foo) where T : class
        {
            if (grid.SelectedItem != null)
                return;
            grid.UnselectAll();
            grid.SelectedItem = FindInTable(data, grid, foo);
        }
        internal void NewSelectInTable<T>(T data, DataGrid grid, Func<T, T, bool> foo) where T : class
        {
            grid.UnselectAll();
            grid.SelectedItem = FindInTable(data, grid, foo);
        }
        internal bool IsOkNum(string val)
        {
            string reg = "[0-9]+";
            return !String.IsNullOrEmpty(val) && Regex.Match(val, reg).Value.Equals(val);
        }
        internal bool IsOkDate(string val)
        {
            string reg = "[0-9]{2}.[0-9]{2}.[0-9]{4}";
            return !String.IsNullOrEmpty(val) && Regex.Match(val, reg).Value.Equals(val);
        }
        internal bool IsOkPhone(string val)
        {
            string reg = "\\+[0-9]{10,13}";
            return !String.IsNullOrEmpty(val) && Regex.Match(val, reg).Value.Equals(val);
        }
        public MainWindow()
        {
            InitializeComponent();
            RefreshTable(supplierTab, new SupplierData(), SupplierTable);
            RefreshTable(detailTab, new DetailData(), DetailTable);
            RefreshTable(deliveryTab, new DeliveryData(), DeliveryTable);
            SupplierTable.IsReadOnly = true;
            DetailTable.IsReadOnly = true;
            DeliveryTable.IsReadOnly = true;
        }

        private void SupplierTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            var dataItem = dataGrid.SelectedItem as SupplierData;
            if(dataItem != null)
            {
                SupplierId.Text = dataItem.Id;
                SupplierName.Text = dataItem.Name;
                SupplierDomain.Text = dataItem.Domain;
                SupplierPhone.Text = dataItem.PhoneNumber;
            }
            SupplierTableChange.IsEnabled = true;
            SupplierTableDelete.IsEnabled = true;
        }
        internal bool SupIdExist(string val)
        {
            bool flag = false;
            foreach (var item in SupplierTable.Items)
            {
                var obj = (SupplierData)item;
                if (obj.Id.Equals(val))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        internal bool IsSupplier(SupplierData sup)
        {
            string temp = "";
            if (!IsOkNum(sup.Id))
                temp += "Неверно введен идентфикатор \n";
            if (!IsOkPhone(sup.PhoneNumber))
                temp += "Неверно введен телефон \n";
            if (String.IsNullOrEmpty(sup.Name))
                temp += "Не введено имя \n";
            if (String.IsNullOrEmpty(sup.Domain))
                temp += "Не введен адрес \n";
            if (temp == "")
                return true;
            MessageBox.Show(temp);
            return false; 
        }
        private void SupplierTableChange_Click(object sender, RoutedEventArgs e)
        {
            var tempForId = SupplierTable.SelectedItem as SupplierData;
            if (SupplierTable.SelectedItem == null && !SupIdExist(SupplierId.Text))
            {
                MessageBox.Show("Нет элемента с таким Id");
                return;
            }
            var obj = new SupplierData(SupplierId.Text, SupplierName.Text, SupplierDomain.Text, SupplierPhone.Text);
            if (!IsSupplier(obj))
                return;
            var list = new List<object>();
            foreach (var item in DeliveryTable.Items)
            {
                var temp = item as DeliveryData;
                if (temp.SupId == tempForId.Id)
                {
                    temp.SupId = SupplierId.Text;
                    list.Add(temp);
                }
            }
            SupplierTableDelete_Click(sender, e);
            SupplierId.Text = obj.Id;
            SupplierName.Text = obj.Name;
            SupplierDomain.Text = obj.Domain;
            SupplierPhone.Text = obj.PhoneNumber;
            foreach (var item in list)
            {
                DeliveryTable.Items.Add(item);
            }
            SupplierTableAdd_Click(sender, e);  
        }

        private void SupplierTableDelete_Click(object sender, RoutedEventArgs e)
        {
            var tempId = SupplierId.Text;
            if (SupplierTable.SelectedItem == null && !SupIdExist(tempId))
            {
                MessageBox.Show("Нет элемента с таким Id");
                return;
            }
            Func<SupplierData, SupplierData, bool> foo = (x, y) => x.Id.Equals(y.Id);
            SelectInTable(new SupplierData(SupplierId.Text, SupplierName.Text, SupplierDomain.Text, SupplierPhone.Text), SupplierTable, foo);
            SupplierTable.Items.Remove(SupplierTable.SelectedItem);
            var list = new List<object>();
            foreach (var item in DeliveryTable.Items)
            {
                var temp = item as DeliveryData;
                if (temp.SupId == tempId)
                    list.Add(item);
            }
            foreach(var item in list)
            {
                DeliveryTable.Items.Remove(item);
            }
            SupplierTableChange.IsEnabled = false;
            SupplierTableDelete.IsEnabled = false;
        }

        private void SupplierTableAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SupIdExist(SupplierId.Text))
            {
                MessageBox.Show("Элемент с таким Id уже есть");
                return;
            }
            Func<SupplierData, SupplierData, bool> foo = (x, y) => x.Id.Equals(y.Id);
            if (!IsSupplier(new SupplierData(SupplierId.Text, SupplierName.Text, SupplierDomain.Text, SupplierPhone.Text)))
                return;
            AddInTable(new SupplierData(), SupplierTable, SupplierId.Text, SupplierName.Text, SupplierDomain.Text, SupplierPhone.Text);
            NewSelectInTable(new SupplierData(SupplierId.Text, SupplierName.Text, SupplierDomain.Text, SupplierPhone.Text), SupplierTable, foo);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SaveTable(supplierTab, new SupplierData(), SupplierTable);
            SaveTable(detailTab, new DetailData(), DetailTable);
            SaveTable(deliveryTab, new DeliveryData(), DeliveryTable);
        }

        private void SupplierId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(SupplierId.Text == "")
            {
                SupplierTableChange.IsEnabled = false;
                SupplierTableDelete.IsEnabled = false;
            }
            else
            {
                SupplierTableChange.IsEnabled = true;
                SupplierTableDelete.IsEnabled = true;
            }
        }

        private void DetailTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            var dataItem = dataGrid.SelectedItem as DetailData;
            if (dataItem != null)
            {
                DetailId.Text = dataItem.Id;
                DetailName.Text = dataItem.Name;
                DetailArticle.Text = dataItem.Article;
                DetailPrice.Text = dataItem.Price;
            }
            DetailTableChange.IsEnabled = true;
            DetailTableDelete.IsEnabled = true;
        }
        internal bool DetIdExist(string val)
        {
            bool flag = false;
            foreach (var item in DetailTable.Items)
            {
                var obj = (DetailData)item;
                if (obj.Id.Equals(val))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        internal bool IsDetail(DetailData sup)
        {
            string temp = "";
            if (!IsOkNum(sup.Id))
                temp += "Неверно введен идентфикатор \n";
            if (!IsOkNum(sup.Price))
                temp += "Неверно введена цена \n";
            if (String.IsNullOrEmpty(sup.Name))
                temp += "Не введено имя \n";
            if (String.IsNullOrEmpty(sup.Article))
                temp += "Не введен артикл \n";
            if (temp == "")
                return true;
            MessageBox.Show(temp);
            return false;
        }

        private void DetailTableChange_Click(object sender, RoutedEventArgs e)
        {
            var tempForId = DetailTable.SelectedItem as DetailData;
            if (DetailTable.SelectedItem == null && !DetIdExist(DetailId.Text))
            {
                MessageBox.Show("Нет элемента с таким Id");
                return;
            }
            var obj = new DetailData(DetailId.Text, DetailName.Text, DetailArticle.Text, DetailPrice.Text);
            if (!IsDetail(obj))
                return;
            var list = new List<object>();
            foreach (var item in DeliveryTable.Items)
            {
                var temp = item as DeliveryData;
                if (temp.DetId == tempForId.Id)
                {
                    temp.DetId = DetailId.Text;
                    list.Add(temp);
                }
            }
            DetailTableDelete_Click(sender, e);
            DetailId.Text = obj.Id;
            DetailName.Text = obj.Name;
            DetailArticle.Text = obj.Article;
            DetailPrice.Text = obj.Price;
            foreach (var item in list)
            {
                DeliveryTable.Items.Add(item);
            }
            DetailTableAdd_Click(sender, e);
        }

        private void DetailTableDelete_Click(object sender, RoutedEventArgs e)
        {
            var tempId = DetailId.Text;
            if (DetailTable.SelectedItem == null && !DetIdExist(tempId))
            {
                MessageBox.Show("Нет элемента с таким Id");
                return;
            }
            Func<DetailData, DetailData, bool> foo = (x, y) => x.Id.Equals(y.Id);
            SelectInTable(new DetailData(DetailId.Text, DetailName.Text, DetailArticle.Text, DetailPrice.Text), DetailTable, foo);
            DetailTable.Items.Remove(DetailTable.SelectedItem);
            var list = new List<object>();
            foreach (var item in DeliveryTable.Items)
            {
                var temp = item as DeliveryData;
                if (temp.DetId == tempId)
                    list.Add(item);
            }
            foreach (var item in list)
            {
                DeliveryTable.Items.Remove(item);
            }
            DetailTableChange.IsEnabled = false;
            DetailTableDelete.IsEnabled = false;
        }

        private void DetailTableAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DetIdExist(DetailId.Text))
            {
                MessageBox.Show("Элемент с таким Id уже есть");
                return;
            }
            Func<DetailData, DetailData, bool> foo = (x, y) => x.Id.Equals(y.Id);
            if (!IsDetail(new DetailData(DetailId.Text, DetailName.Text, DetailArticle.Text, DetailPrice.Text)))
                return;
            AddInTable(new DetailData(), DetailTable, DetailId.Text, DetailName.Text, DetailArticle.Text, DetailPrice.Text);
            NewSelectInTable(new DetailData(DetailId.Text, DetailName.Text, DetailArticle.Text, DetailPrice.Text), DetailTable, foo);
        }

        private void DetailId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DetailId.Text == "")
            {
                DetailTableChange.IsEnabled = false;
                DetailTableDelete.IsEnabled = false;
            }
            else
            {
                DetailTableChange.IsEnabled = true;
                DetailTableDelete.IsEnabled = true;
            }
        }

        private void DeliveryTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            var dataItem = dataGrid.SelectedItem as DeliveryData;
            if (dataItem != null)
            {
                DeliverySupId.Text = dataItem.SupId;
                DeliveryDetId.Text = dataItem.DetId;
                DeliveryCount.Text = dataItem.Count;
                DeliveryData.Text = dataItem.Data;
            }
            DeliveryTableChange.IsEnabled = true;
            DeliveryTableDelete.IsEnabled = true;
        }
        
        internal bool IsDelivery(DeliveryData sup)
        {
            string temp = "";
            if (!IsOkNum(sup.SupId))
                temp += "Неверно введен идентфикатор поставщика \n";
            if (!IsOkNum(sup.DetId))
                temp += "Неверно введен идентификатор товара \n";
            if (!IsOkNum(sup.Count))
                temp += "Не введено количество \n";
            DateTime d;
            if (!DateTime.TryParseExact(sup.Data, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                temp += "Неверно введена дата \n";
            if (temp == "")
                return true;
            MessageBox.Show(temp);
            return false;
        }

        private void DeliveryTableChange_Click(object sender, RoutedEventArgs e)
        {
            string debug = "";
            if (!SupIdExist(DeliverySupId.Text))
                debug += "Нет такого поставщика \n";
            if (!DetIdExist(DeliveryDetId.Text))
                debug += "Нет такой детали \n";
            if (debug != "")
            {
                MessageBox.Show(debug);
                return;
            }
            var obj = new DeliveryData(DeliverySupId.Text, DeliveryDetId.Text, DeliveryCount.Text, DeliveryData.Text);
            if (!IsDelivery(obj))
                return;
            DeliveryTableDelete_Click(sender, e);
            DeliverySupId.Text = obj.SupId;
            DeliveryDetId.Text = obj.DetId;
            DeliveryCount.Text = obj.Count;
            DeliveryData.Text = obj.Data;
            DeliveryTableAdd_Click(sender, e);
        }

        private void DeliveryTableDelete_Click(object sender, RoutedEventArgs e)
        {
            string debug = "";
            if (DeliveryTable.SelectedItem == null && !SupIdExist(DeliverySupId.Text))
                debug += "Нет такого поставщика \n";
            if (DeliveryTable.SelectedItem == null && !DetIdExist(DeliveryDetId.Text))
                debug += "Нет такой детали \n";
            if (debug != "")
            {
                MessageBox.Show(debug);
                return;
            }
            Func<DeliveryData, DeliveryData, bool> foo = (x, y) => x.SupId.Equals(y.SupId) && x.DetId.Equals(y.DetId);
            SelectInTable(new DeliveryData(DeliverySupId.Text, DeliveryDetId.Text, DeliveryCount.Text, DeliveryData.Text), DeliveryTable, foo);
            DeliveryTable.Items.Remove(DeliveryTable.SelectedItem);
            DeliveryTableChange.IsEnabled = false;
            DeliveryTableDelete.IsEnabled = false;
        }

        private void DeliveryTableAdd_Click(object sender, RoutedEventArgs e)
        {
            string debug = "";
            if (!SupIdExist(DeliverySupId.Text))
                debug += "Нет такого поставщика \n";
            if (!DetIdExist(DeliveryDetId.Text))
                debug += "Нет такой детали \n";
            if (debug != "")
            {
                MessageBox.Show(debug);
                return;
            }
            Func<DeliveryData, DeliveryData, bool> foo = (x, y) => x.SupId.Equals(y.SupId) && x.DetId.Equals(y.DetId);
            if (!IsDelivery(new DeliveryData(DeliverySupId.Text, DeliveryDetId.Text, DeliveryCount.Text, DeliveryData.Text)))
                return;
            AddInTable(new DeliveryData(), DeliveryTable, DeliverySupId.Text, DeliveryDetId.Text, DeliveryCount.Text, DeliveryData.Text);
            NewSelectInTable(new DeliveryData(DeliverySupId.Text, DeliveryDetId.Text, DeliveryCount.Text, DeliveryData.Text), DeliveryTable, foo);
        }

        private void DeliverySupId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DeliveryDetId.Text == "" || DeliverySupId.Text == "")
            {
                DeliveryTableChange.IsEnabled = false;
                DeliveryTableDelete.IsEnabled = false;
            }
            else
            {
                DeliveryTableChange.IsEnabled = true;
                DeliveryTableDelete.IsEnabled = true;
            }
        }

        private void DeliveryDetId_TextChanged(object sender, TextChangedEventArgs e)
        {
            DeliverySupId_TextChanged(sender, e);
        }

        private void CountInData_Click(object sender, RoutedEventArgs e)
        {
            var win = new CountDate
            {
                Owner = this
            };
            win.Show();
        }

        private void Prices_Click(object sender, RoutedEventArgs e)
        {
            var win = new AllTotalCounts(this);
            win.Show();
        }
    }
}
