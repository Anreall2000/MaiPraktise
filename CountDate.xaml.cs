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
    /// Логика взаимодействия для CountDate.xaml
    /// </summary>
    public partial class CountDate : Window
    {
        public CountDate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var temp = Owner as MainWindow;
            uint count = 0;
            foreach (var item in temp.DeliveryTable.Items)
            {
                var temp2 = item as DeliveryData;
                if(DateTime.Parse(temp2.Data) > DateTime.Parse(From.Text) && DateTime.Parse(temp2.Data) < DateTime.Parse(To.Text))
                {
                    count += uint.Parse(temp2.Count);
                }
            }
            
            Text.Content = "Введите промежуток времени\n" + "Количество деталей:" + count.ToString();
        }
    }
}
