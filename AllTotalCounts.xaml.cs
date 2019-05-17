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
using System.Windows.Shapes;

namespace DumbSql
{
    /// <summary>
    /// Логика взаимодействия для AllTotalCounts.xaml
    /// </summary>
    internal class TotalCountsData
    {
        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string SinglPrice { get => singlePrice; set => singlePrice = value; }
        public string TotalCount { get => totalCount; set => totalCount = value; }
        public string TotalPrice { get => (int.Parse(totalCount)*int.Parse(singlePrice)).ToString(); }

        public TotalCountsData() { }
        public TotalCountsData (params string[] args)
        {
            Id = args[0];
            Name = args[1];
            SinglPrice = args[2];
            TotalCount = args[3];
        }

        private string totalCount;
        private string singlePrice;
        private string id;
        private string name;
    }
    public partial class AllTotalCounts : Window
    {
        public AllTotalCounts(MainWindow main)
        {
            
            InitializeComponent();
            var temp = main;
            var map = new Dictionary<int, TotalCountsData>();
            foreach(var item in temp.DetailTable.Items)
            {
                var temp2 = item as DetailData;
                map[int.Parse(temp2.Id)] = new TotalCountsData(temp2.Id, temp2.Name, temp2.Price, "0");
            }
            foreach (var item in temp.DeliveryTable.Items)
            {
                var temp2 = item as DeliveryData;
                if (map.ContainsKey(int.Parse(temp2.DetId)))
                {
                    map[int.Parse(temp2.DetId)].TotalCount = (int.Parse(temp2.Count) + int.Parse(map[int.Parse(temp2.DetId)].TotalCount)).ToString();
                }
            }
            foreach (var key in map.Keys)
            {
                PricesGrid.Items.Add(map[key]);
            }
        }
    }
}
